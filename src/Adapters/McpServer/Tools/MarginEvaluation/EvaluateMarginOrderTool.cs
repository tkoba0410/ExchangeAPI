using System.Globalization;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.MarginEvaluation;

public sealed class EvaluateMarginOrderTool
{
    private const string SupportedSymbol = "FX_BTC_JPY";

    private readonly IBitflyerEvaluateMarginOrderGateway _gateway;
    private readonly EvaluateMarginOrderOptions _options;

    public EvaluateMarginOrderTool(
        IBitflyerEvaluateMarginOrderGateway gateway,
        EvaluateMarginOrderOptions? options = null)
    {
        _gateway = gateway;
        _options = options ?? new EvaluateMarginOrderOptions();
        ValidateOptions(_options);
    }

    public async Task<McpToolExecutionResult<EvaluateMarginOrderResponse>> ExecuteAsync(
        EvaluateMarginOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (validation.Error is not null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(validation.Error);
        }

        var normalized = validation.Value!;
        if (!BitflyerMarginRuleRegistry.TryGet(normalized.Symbol, out var rule) || rule is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                DomainError(
                    errorCode: "invalid_market_rule",
                    message: "Supported symbol is missing required margin rules.",
                    details: new Dictionary<string, string?> { ["symbol"] = normalized.Symbol }));
        }

        if (!TryParseRule(rule, out var parsedRule, out var ruleError))
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(ruleError!);
        }

        var tickerTask = _gateway.GetTickerCallAsync(normalized.Symbol, cancellationToken);
        var boardStateTask = _gateway.GetBoardStateCallAsync(normalized.Symbol, cancellationToken);
        var collateralTask = _gateway.GetCollateralCallAsync(cancellationToken);
        var positionsTask = _gateway.GetPositionsCallAsync(normalized.Symbol, cancellationToken);
        var activeOrdersTask = _gateway.GetActiveChildOrdersCallAsync(normalized.Symbol, cancellationToken);
        var leverageTask = _gateway.GetCorporateLeverageCallAsync(cancellationToken);

        await Task.WhenAll(tickerTask, boardStateTask, collateralTask, positionsTask, activeOrdersTask, leverageTask);

        var tickerCall = await tickerTask;
        if (!tickerCall.IsSuccess || tickerCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("market_unavailable", "Failed to load ticker from upstream.", "GetTicker", normalized.Symbol, tickerCall.Error));
        }

        var boardStateCall = await boardStateTask;
        if (!boardStateCall.IsSuccess || boardStateCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("market_unavailable", "Failed to load board state from upstream.", "GetBoardState", normalized.Symbol, boardStateCall.Error));
        }

        var collateralCall = await collateralTask;
        if (!collateralCall.IsSuccess || collateralCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("account_unavailable", "Failed to load collateral from upstream.", "GetCollateral", normalized.Symbol, collateralCall.Error));
        }

        var positionsCall = await positionsTask;
        if (!positionsCall.IsSuccess || positionsCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("account_unavailable", "Failed to load positions from upstream.", "GetPositions", normalized.Symbol, positionsCall.Error));
        }

        var activeOrdersCall = await activeOrdersTask;
        if (!activeOrdersCall.IsSuccess || activeOrdersCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("account_unavailable", "Failed to load active child orders from upstream.", "GetChildOrders", normalized.Symbol, activeOrdersCall.Error));
        }

        var leverageCall = await leverageTask;
        if (!leverageCall.IsSuccess || leverageCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                UpstreamError("market_unavailable", "Failed to load corporate leverage from upstream.", "GetCorporateLeverage", normalized.Symbol, leverageCall.Error));
        }

        if (leverageCall.Response.CurrentMax <= 0m)
        {
            return McpToolExecutionResult<EvaluateMarginOrderResponse>.Failure(
                DomainError(
                    errorCode: "invalid_market_rule",
                    message: "Corporate leverage response returned a non-positive current max leverage.",
                    details: new Dictionary<string, string?>
                    {
                        ["symbol"] = normalized.Symbol,
                        ["currentMaxLeverage"] = FormatDecimal(leverageCall.Response.CurrentMax),
                    }));
        }

        var referencePrice = SelectReferencePrice(normalized, tickerCall.Response);
        var estimatedNotional = referencePrice * normalized.SizeValue;
        var estimatedRequiredCollateral = estimatedNotional / leverageCall.Response.CurrentMax;
        var estimatedFee = EstimateFee(normalized.OrderType, estimatedNotional);
        var derivedAvailable = collateralCall.Response.Collateral
            + collateralCall.Response.OpenPositionPnl
            - collateralCall.Response.RequireCollateral;
        var marketStatus = BitflyerMarketStatusMapper.Map(boardStateCall.Response.State, boardStateCall.Response.Health);
        bool? feeCoverageOk = estimatedFee.HasValue
            ? derivedAvailable >= estimatedRequiredCollateral + estimatedFee.Value
            : null;

        var checks = new EvaluateMarginOrderChecks
        {
            SymbolOk = true,
            MarketStatusOk = string.Equals(marketStatus, "active", StringComparison.Ordinal),
            SizeRuleOk = IsSizeRuleOk(normalized.SizeValue, parsedRule.MinSize, parsedRule.SizeStep),
            PriceRuleOk = normalized.OrderType == "market" || IsPriceRuleOk(normalized.PriceValue!.Value, parsedRule.PriceStep),
            CollateralCoverageOk = derivedAvailable >= estimatedRequiredCollateral,
            FeeCoverageOk = feeCoverageOk,
            ProjectedMarginExposureOk = IsProjectedMarginExposureOk(normalized, activeOrdersCall.Response, positionsCall.Response),
            CurrentMaintenanceOk = collateralCall.Response.KeepRate >= parsedRule.MinimumKeepRate,
        };

        var response = new EvaluateMarginOrderResponse
        {
            CanPlace = checks.SymbolOk
                && checks.MarketStatusOk
                && checks.SizeRuleOk
                && checks.PriceRuleOk
                && checks.CollateralCoverageOk
                && checks.ProjectedMarginExposureOk
                && checks.CurrentMaintenanceOk,
            Checks = checks,
            NormalizedRequest = new EvaluateMarginOrderRequest
            {
                Venue = normalized.Venue,
                AccountContext = normalized.AccountContext,
                Symbol = normalized.Symbol,
                Side = normalized.Side,
                OrderType = normalized.OrderType,
                Size = FormatDecimal(normalized.SizeValue),
                Price = normalized.PriceValue is decimal price ? FormatDecimal(price) : null,
            },
            Estimate = new EvaluateMarginOrderEstimate
            {
                ReferencePrice = FormatDecimal(referencePrice),
                EstimatedNotional = FormatDecimal(estimatedNotional),
                EstimatedRequiredCollateral = FormatDecimal(estimatedRequiredCollateral),
                CurrentMaxLeverage = FormatDecimal(leverageCall.Response.CurrentMax),
                CurrentKeepRate = FormatDecimal(collateralCall.Response.KeepRate),
                MinimumKeepRate = FormatDecimal(parsedRule.MinimumKeepRate),
                EstimatedFee = estimatedFee is decimal fee ? FormatDecimal(fee) : null,
                EstimatedFeeSourceKind = estimatedFee.HasValue ? MarketRuleSourceKinds.PinnedOperational : null,
            },
            Warnings = BuildWarnings(normalized.OrderType, feeCoverageOk),
            Reasons = BuildReasons(checks),
        };

        return McpToolExecutionResult<EvaluateMarginOrderResponse>.Success(response);
    }

    private static ValidationResult Validate(EvaluateMarginOrderRequest request)
    {
        if (!BitflyerPrivateContextValidator.TryNormalize(
                request.Venue,
                request.AccountContext,
                out var normalizedVenue,
                out var normalizedAccountContext,
                out var contextError))
        {
            return ValidationResult.Fail(contextError!);
        }

        var symbol = request.Symbol.Trim().ToUpperInvariant();
        if (!string.Equals(symbol, SupportedSymbol, StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_symbol",
                    "Unsupported symbol.",
                    new Dictionary<string, string?> { ["symbol"] = symbol }));
        }

        var side = request.Side.Trim().ToLowerInvariant();
        if (!string.Equals(side, "buy", StringComparison.Ordinal) &&
            !string.Equals(side, "sell", StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_side",
                    "Side must be buy or sell.",
                    new Dictionary<string, string?> { ["side"] = request.Side }));
        }

        var orderType = request.OrderType.Trim().ToLowerInvariant();
        if (!string.Equals(orderType, "market", StringComparison.Ordinal) &&
            !string.Equals(orderType, "limit", StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_order_type",
                    "Order type must be market or limit.",
                    new Dictionary<string, string?> { ["orderType"] = request.OrderType }));
        }

        if (!TryParsePositiveDecimal(request.Size, out var sizeValue))
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_size",
                    "Size must be a positive decimal string.",
                    new Dictionary<string, string?> { ["size"] = request.Size }));
        }

        if (string.Equals(orderType, "market", StringComparison.Ordinal))
        {
            if (request.Price is not null)
            {
                return ValidationResult.Fail(
                    ValidationError(
                        "invalid_price",
                        "Price must be omitted for market orders.",
                        new Dictionary<string, string?> { ["price"] = request.Price }));
            }

            return ValidationResult.Ok(new NormalizedRequest(normalizedVenue, normalizedAccountContext, symbol, side, orderType, sizeValue, null));
        }

        if (request.Price is null)
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_price",
                    "Price is required for limit orders.",
                    new Dictionary<string, string?>()));
        }

        if (!TryParsePositiveDecimal(request.Price, out var priceValue))
        {
            return ValidationResult.Fail(
                ValidationError(
                    "invalid_price",
                    "Price must be a positive decimal string.",
                    new Dictionary<string, string?> { ["price"] = request.Price }));
        }

        return ValidationResult.Ok(new NormalizedRequest(normalizedVenue, normalizedAccountContext, symbol, side, orderType, sizeValue, priceValue));
    }

    private static bool TryParseRule(
        BitflyerMarginRule rule,
        out ParsedRule parsedRule,
        out McpToolError? error)
    {
        if (!TryParsePositiveDecimal(rule.MinSize, out var minSize) ||
            !TryParsePositiveDecimal(rule.SizeStep, out var sizeStep) ||
            !TryParsePositiveDecimal(rule.PriceStep, out var priceStep) ||
            !TryParsePositiveDecimal(rule.MinimumKeepRate, out var minimumKeepRate))
        {
            parsedRule = default;
            error = DomainError(
                "invalid_market_rule",
                "Margin rule registry contains an invalid decimal.",
                new Dictionary<string, string?>
                {
                    ["symbol"] = rule.Symbol,
                    ["minSize"] = rule.MinSize,
                    ["sizeStep"] = rule.SizeStep,
                    ["priceStep"] = rule.PriceStep,
                    ["minimumKeepRate"] = rule.MinimumKeepRate,
                });
            return false;
        }

        parsedRule = new ParsedRule(minSize, sizeStep, priceStep, minimumKeepRate);
        error = null;
        return true;
    }

    private static bool TryParsePositiveDecimal(string value, out decimal result)
    {
        return decimal.TryParse(
                value,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out result)
            && result > 0m;
    }

    private static decimal SelectReferencePrice(NormalizedRequest request, GetTickerResponse ticker)
    {
        if (string.Equals(request.OrderType, "limit", StringComparison.Ordinal))
        {
            return request.PriceValue!.Value;
        }

        return string.Equals(request.Side, "buy", StringComparison.Ordinal)
            ? ticker.BestAsk
            : ticker.BestBid;
    }

    private static bool IsSizeRuleOk(decimal size, decimal minSize, decimal sizeStep)
    {
        return size >= minSize && decimal.Remainder(size, sizeStep) == 0m;
    }

    private static bool IsPriceRuleOk(decimal price, decimal priceStep)
    {
        return price > 0m && decimal.Remainder(price, priceStep) == 0m;
    }

    private bool IsProjectedMarginExposureOk(
        NormalizedRequest request,
        IReadOnlyList<GetChildOrders.Item> activeOrders,
        IReadOnlyList<GetPositions.Item> positions)
    {
        if (_options.MaxBaseSize is not decimal maxBaseSize)
        {
            return true;
        }

        var activeSameSideSize = activeOrders
            .Where(item => string.Equals(item.ProductCode, request.Symbol, StringComparison.Ordinal)
                && string.Equals(MapSide(item.Side), request.Side, StringComparison.Ordinal)
                && item.ChildOrderState == ChildOrderStates.Active)
            .Sum(item => item.OutstandingSize);

        var positionSameSideSize = positions
            .Where(item => string.Equals(item.ProductCode, request.Symbol, StringComparison.Ordinal)
                && string.Equals(MapSide(item.Side), request.Side, StringComparison.Ordinal))
            .Sum(item => item.Size);

        return activeSameSideSize + positionSameSideSize + request.SizeValue <= maxBaseSize;
    }

    private decimal? EstimateFee(string orderType, decimal estimatedNotional)
    {
        var feeRate = string.Equals(orderType, "market", StringComparison.Ordinal)
            ? _options.MarketFeeRate
            : _options.LimitFeeRate;

        return feeRate is decimal configuredRate
            ? estimatedNotional * configuredRate
            : null;
    }

    private static IReadOnlyList<string> BuildWarnings(string orderType, bool? feeCoverageOk)
    {
        var warnings = new List<string>();
        if (string.Equals(orderType, "market", StringComparison.Ordinal))
        {
            warnings.Add(EvaluateMarginOrderWarningCodes.MarketOrderSlippageRisk);
        }

        if (feeCoverageOk == false)
        {
            warnings.Add(EvaluateMarginOrderWarningCodes.EstimatedFeeNotCovered);
        }

        return warnings;
    }

    private static IReadOnlyList<string> BuildReasons(EvaluateMarginOrderChecks checks)
    {
        var reasons = new List<string>();
        if (!checks.MarketStatusOk)
        {
            reasons.Add("market_not_active");
        }

        if (!checks.SizeRuleOk)
        {
            reasons.Add("size_rule_violation");
        }

        if (!checks.PriceRuleOk)
        {
            reasons.Add("price_rule_violation");
        }

        if (!checks.CollateralCoverageOk)
        {
            reasons.Add("insufficient_collateral");
        }

        if (!checks.ProjectedMarginExposureOk)
        {
            reasons.Add("exposure_limit_exceeded");
        }

        if (!checks.CurrentMaintenanceOk)
        {
            reasons.Add("maintenance_not_safe");
        }

        return reasons;
    }

    private static string MapSide(BitflyerOrderSide side)
    {
        return side switch
        {
            OrderSides.Buy => "buy",
            OrderSides.Sell => "sell",
            _ => throw new InvalidOperationException($"Unsupported bitFlyer order side: {side}."),
        };
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static McpToolError ValidationError(
        string errorCode,
        string message,
        IReadOnlyDictionary<string, string?> details)
    {
        return new McpToolError
        {
            ErrorCategory = "validation_error",
            ErrorCode = errorCode,
            Message = message,
            Details = details,
            Retryable = false,
        };
    }

    private static McpToolError DomainError(
        string errorCode,
        string message,
        IReadOnlyDictionary<string, string?> details)
    {
        return new McpToolError
        {
            ErrorCategory = "domain_error",
            ErrorCode = errorCode,
            Message = message,
            Details = details,
            Retryable = false,
        };
    }

    private static McpToolError UpstreamError(
        string errorCode,
        string message,
        string endpoint,
        string symbol,
        CallError? error)
    {
        return new McpToolError
        {
            ErrorCategory = "upstream_error",
            ErrorCode = errorCode,
            Message = message,
            Details = new Dictionary<string, string?>
            {
                ["endpoint"] = endpoint,
                ["symbol"] = symbol,
                ["callErrorKind"] = error?.Kind,
                ["callErrorMessage"] = error?.Message,
            },
            Retryable = true,
        };
    }

    private static void ValidateOptions(EvaluateMarginOrderOptions options)
    {
        if (options.MaxBaseSize is decimal maxBaseSize && maxBaseSize <= 0m)
        {
            throw new InvalidOperationException("EvaluateMarginOrderOptions.MaxBaseSize must be positive when configured.");
        }

        if (options.MarketFeeRate is decimal marketFeeRate && marketFeeRate < 0m)
        {
            throw new InvalidOperationException("EvaluateMarginOrderOptions.MarketFeeRate must be non-negative when configured.");
        }

        if (options.LimitFeeRate is decimal limitFeeRate && limitFeeRate < 0m)
        {
            throw new InvalidOperationException("EvaluateMarginOrderOptions.LimitFeeRate must be non-negative when configured.");
        }
    }

    private sealed record NormalizedRequest(
        string Venue,
        string AccountContext,
        string Symbol,
        string Side,
        string OrderType,
        decimal SizeValue,
        decimal? PriceValue);

    private readonly record struct ParsedRule(
        decimal MinSize,
        decimal SizeStep,
        decimal PriceStep,
        decimal MinimumKeepRate);

    private sealed class ValidationResult
    {
        public NormalizedRequest? Value { get; private init; }

        public McpToolError? Error { get; private init; }

        public static ValidationResult Ok(NormalizedRequest value) => new() { Value = value };

        public static ValidationResult Fail(McpToolError error) => new() { Error = error };
    }
}
