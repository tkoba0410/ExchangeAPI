using System.Globalization;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Evaluation;

public sealed class EvaluateOrderTool
{
    private const string SupportedSymbol = "BTC_JPY";
    private const string JpyCurrencyCode = "JPY";
    private const string BtcCurrencyCode = "BTC";

    private readonly IBitflyerEvaluateOrderGateway _gateway;
    private readonly EvaluateOrderOptions _options;

    public EvaluateOrderTool(
        IBitflyerEvaluateOrderGateway gateway,
        EvaluateOrderOptions? options = null)
    {
        _gateway = gateway;
        _options = options ?? new EvaluateOrderOptions();
        ValidateOptions(_options);
    }

    public async Task<McpToolExecutionResult<EvaluateOrderResponse>> ExecuteAsync(
        EvaluateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (validation.Error is not null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(validation.Error);
        }

        var normalized = validation.Value!;
        if (!BitflyerMarketRuleRegistry.TryGet(normalized.Symbol, out var rule) || rule is null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(
                DomainError(
                    errorCode: "invalid_market_rule",
                    message: "Supported symbol is missing required market rules.",
                    details: new Dictionary<string, string?> { ["symbol"] = normalized.Symbol }));
        }

        if (!TryParseRule(rule, out var parsedRule, out var ruleError))
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(ruleError!);
        }

        var tickerTask = _gateway.GetTickerCallAsync(normalized.Symbol, cancellationToken);
        var boardStateTask = _gateway.GetBoardStateCallAsync(normalized.Symbol, cancellationToken);
        var balanceTask = _gateway.GetBalanceCallAsync(cancellationToken);
        var activeOrdersTask = _gateway.GetActiveChildOrdersCallAsync(normalized.Symbol, cancellationToken);

        await Task.WhenAll(tickerTask, boardStateTask, balanceTask, activeOrdersTask);

        var tickerCall = await tickerTask;
        if (!tickerCall.IsSuccess || tickerCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(
                UpstreamError(
                    errorCode: "market_unavailable",
                    message: "Failed to load ticker from upstream.",
                    endpoint: "GetTicker",
                    symbol: normalized.Symbol,
                    error: tickerCall.Error));
        }

        var boardStateCall = await boardStateTask;
        if (!boardStateCall.IsSuccess || boardStateCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(
                UpstreamError(
                    errorCode: "market_unavailable",
                    message: "Failed to load board state from upstream.",
                    endpoint: "GetBoardState",
                    symbol: normalized.Symbol,
                    error: boardStateCall.Error));
        }

        var balanceCall = await balanceTask;
        if (!balanceCall.IsSuccess || balanceCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(
                UpstreamError(
                    errorCode: "account_unavailable",
                    message: "Failed to load balance from upstream.",
                    endpoint: "GetBalance",
                    symbol: normalized.Symbol,
                    error: balanceCall.Error));
        }

        var activeOrdersCall = await activeOrdersTask;
        if (!activeOrdersCall.IsSuccess || activeOrdersCall.Response is null)
        {
            return McpToolExecutionResult<EvaluateOrderResponse>.Failure(
                UpstreamError(
                    errorCode: "account_unavailable",
                    message: "Failed to load active child orders from upstream.",
                    endpoint: "GetChildOrders",
                    symbol: normalized.Symbol,
                    error: activeOrdersCall.Error));
        }

        var referencePrice = SelectReferencePrice(normalized, tickerCall.Response);
        var estimatedNotional = referencePrice * normalized.SizeValue;
        var estimatedFee = EstimateFee(normalized.OrderType, estimatedNotional);
        var marketStatus = BitflyerMarketStatusMapper.Map(boardStateCall.Response.State, boardStateCall.Response.Health);
        var feeCoverageOk = IsFeeCoverageOk(normalized, estimatedNotional, estimatedFee, balanceCall.Response);

        var checks = new EvaluateOrderChecks
        {
            SymbolOk = true,
            MarketStatusOk = string.Equals(marketStatus, "active", StringComparison.Ordinal),
            SizeRuleOk = IsSizeRuleOk(normalized.SizeValue, parsedRule.MinSize, parsedRule.SizeStep),
            PriceRuleOk = normalized.OrderType == "market" || IsPriceRuleOk(normalized.PriceValue!.Value, parsedRule.PriceStep),
            BalanceOk = IsBalanceOk(normalized, estimatedNotional, balanceCall.Response),
            FeeCoverageOk = feeCoverageOk,
            ProjectedExposureOk = IsProjectedExposureOk(normalized, activeOrdersCall.Response),
        };

        var reasons = BuildReasons(checks);
        var warnings = BuildWarnings(normalized.OrderType, feeCoverageOk);

        var response = new EvaluateOrderResponse
        {
            CanPlace = checks.SymbolOk
                && checks.MarketStatusOk
                && checks.SizeRuleOk
                && checks.PriceRuleOk
                && checks.BalanceOk
                && checks.ProjectedExposureOk,
            Checks = checks,
            NormalizedRequest = new EvaluateOrderRequest
            {
                Venue = normalized.Venue,
                AccountContext = normalized.AccountContext,
                Symbol = normalized.Symbol,
                Side = normalized.Side,
                OrderType = normalized.OrderType,
                Size = FormatDecimal(normalized.SizeValue),
                Price = normalized.PriceValue is decimal price ? FormatDecimal(price) : null,
            },
            Estimate = new EvaluateOrderEstimate
            {
                ReferencePrice = FormatDecimal(referencePrice),
                EstimatedNotional = FormatDecimal(estimatedNotional),
                EstimatedFee = estimatedFee is decimal fee ? FormatDecimal(fee) : null,
                EstimatedFeeSourceKind = estimatedFee.HasValue ? MarketRuleSourceKinds.PinnedOperational : null,
            },
            Warnings = warnings,
            Reasons = reasons,
        };

        return McpToolExecutionResult<EvaluateOrderResponse>.Success(response);
    }

    private static ValidationResult Validate(EvaluateOrderRequest request)
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
                    errorCode: "invalid_symbol",
                    message: "Unsupported symbol.",
                    details: new Dictionary<string, string?> { ["symbol"] = symbol }));
        }

        var side = request.Side.Trim().ToLowerInvariant();
        if (!string.Equals(side, "buy", StringComparison.Ordinal) &&
            !string.Equals(side, "sell", StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_side",
                    message: "Side must be buy or sell.",
                    details: new Dictionary<string, string?> { ["side"] = request.Side }));
        }

        var orderType = request.OrderType.Trim().ToLowerInvariant();
        if (!string.Equals(orderType, "market", StringComparison.Ordinal) &&
            !string.Equals(orderType, "limit", StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_order_type",
                    message: "Order type must be market or limit.",
                    details: new Dictionary<string, string?> { ["orderType"] = request.OrderType }));
        }

        if (!TryParsePositiveDecimal(request.Size, out var sizeValue))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_size",
                    message: "Size must be a positive decimal string.",
                    details: new Dictionary<string, string?> { ["size"] = request.Size }));
        }

        if (string.Equals(orderType, "market", StringComparison.Ordinal))
        {
            if (request.Price is not null)
            {
                return ValidationResult.Fail(
                    ValidationError(
                        errorCode: "invalid_price",
                        message: "Price must be omitted for market orders.",
                        details: new Dictionary<string, string?> { ["price"] = request.Price }));
            }

            return ValidationResult.Ok(new NormalizedEvaluateOrderRequest(normalizedVenue, normalizedAccountContext, symbol, side, orderType, sizeValue, null));
        }

        if (request.Price is null)
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_price",
                    message: "Price is required for limit orders.",
                    details: new Dictionary<string, string?>()));
        }

        if (!TryParsePositiveDecimal(request.Price, out var priceValue))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_price",
                    message: "Price must be a positive decimal string.",
                    details: new Dictionary<string, string?> { ["price"] = request.Price }));
        }

        return ValidationResult.Ok(new NormalizedEvaluateOrderRequest(normalizedVenue, normalizedAccountContext, symbol, side, orderType, sizeValue, priceValue));
    }

    private static bool TryParseRule(
        BitflyerMarketRule rule,
        out ParsedRule parsedRule,
        out McpToolError? error)
    {
        if (!TryParsePositiveDecimal(rule.MinSize, out var minSize) ||
            !TryParsePositiveDecimal(rule.SizeStep, out var sizeStep) ||
            !TryParsePositiveDecimal(rule.PriceStep, out var priceStep))
        {
            parsedRule = default;
            error = DomainError(
                errorCode: "invalid_market_rule",
                message: "Market rule registry contains an invalid decimal.",
                details: new Dictionary<string, string?>
                {
                    ["symbol"] = rule.Symbol,
                    ["minSize"] = rule.MinSize,
                    ["sizeStep"] = rule.SizeStep,
                    ["priceStep"] = rule.PriceStep,
                });
            return false;
        }

        parsedRule = new ParsedRule(minSize, sizeStep, priceStep);
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

    private static decimal SelectReferencePrice(
        NormalizedEvaluateOrderRequest request,
        GetTickerResponse ticker)
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

    private static bool IsBalanceOk(
        NormalizedEvaluateOrderRequest request,
        decimal estimatedNotional,
        IReadOnlyList<GetBalance.Item> balances)
    {
        var availableByCurrency = balances.ToDictionary(
            item => item.CurrencyCode,
            item => item.Available,
            StringComparer.Ordinal);

        if (string.Equals(request.Side, "buy", StringComparison.Ordinal))
        {
            return availableByCurrency.GetValueOrDefault(JpyCurrencyCode, 0m) >= estimatedNotional;
        }

        return availableByCurrency.GetValueOrDefault(BtcCurrencyCode, 0m) >= request.SizeValue;
    }

    private decimal? EstimateFee(string orderType, decimal estimatedNotional)
    {
        var feeRate = string.Equals(orderType, "market", StringComparison.Ordinal)
            ? _options.MarketFeeRate
            : _options.LimitFeeRate;

        return feeRate.HasValue
            ? estimatedNotional * feeRate.Value
            : null;
    }

    private static bool? IsFeeCoverageOk(
        NormalizedEvaluateOrderRequest request,
        decimal estimatedNotional,
        decimal? estimatedFee,
        IReadOnlyList<GetBalance.Item> balances)
    {
        if (!estimatedFee.HasValue)
        {
            return null;
        }

        if (!string.Equals(request.Side, "buy", StringComparison.Ordinal))
        {
            return null;
        }

        var availableByCurrency = balances.ToDictionary(
            item => item.CurrencyCode,
            item => item.Available,
            StringComparer.Ordinal);

        return availableByCurrency.GetValueOrDefault(JpyCurrencyCode, 0m) >= estimatedNotional + estimatedFee.Value;
    }

    private bool IsProjectedExposureOk(
        NormalizedEvaluateOrderRequest request,
        IReadOnlyList<GetChildOrders.Item> activeOrders)
    {
        if (!_options.MaxBaseSize.HasValue)
        {
            return true;
        }

        var outstandingExposure = activeOrders
            .Where(item =>
                string.Equals(item.ProductCode, request.Symbol, StringComparison.Ordinal)
                && item.ChildOrderState == ChildOrderStates.Active
                && IsSameSide(item.Side, request.Side))
            .Sum(item => item.OutstandingSize);

        return outstandingExposure + request.SizeValue <= _options.MaxBaseSize.Value;
    }

    private static bool IsSameSide(BitflyerOrderSide side, string normalizedSide)
    {
        return side switch
        {
            _ when string.Equals(normalizedSide, "buy", StringComparison.Ordinal) => side == OrderSides.Buy,
            _ when string.Equals(normalizedSide, "sell", StringComparison.Ordinal) => side == OrderSides.Sell,
            _ => false,
        };
    }

    private static IReadOnlyList<string> BuildReasons(EvaluateOrderChecks checks)
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

        if (!checks.BalanceOk)
        {
            reasons.Add("insufficient_balance");
        }

        if (!checks.ProjectedExposureOk)
        {
            reasons.Add("exposure_limit_exceeded");
        }

        return reasons;
    }

    private static IReadOnlyList<string> BuildWarnings(string orderType, bool? feeCoverageOk)
    {
        var warnings = new List<string>();
        if (string.Equals(orderType, "market", StringComparison.Ordinal))
        {
            warnings.Add(EvaluateOrderWarningCodes.MarketOrderSlippageRisk);
        }

        if (feeCoverageOk == false)
        {
            warnings.Add(EvaluateOrderWarningCodes.EstimatedFeeNotCovered);
        }

        return warnings;
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

    private static void ValidateOptions(EvaluateOrderOptions options)
    {
        ValidateFeeRate(options.MarketFeeRate, nameof(EvaluateOrderOptions.MarketFeeRate));
        ValidateFeeRate(options.LimitFeeRate, nameof(EvaluateOrderOptions.LimitFeeRate));
    }

    private static void ValidateFeeRate(decimal? rate, string name)
    {
        if (!rate.HasValue)
        {
            return;
        }

        if (rate.Value < 0m || rate.Value > 1m)
        {
            throw new ArgumentOutOfRangeException(name, "Fee rate must be between 0 and 1.");
        }
    }

    private sealed record NormalizedEvaluateOrderRequest(
        string Venue,
        string AccountContext,
        string Symbol,
        string Side,
        string OrderType,
        decimal SizeValue,
        decimal? PriceValue);

    private readonly record struct ParsedRule(decimal MinSize, decimal SizeStep, decimal PriceStep);

    private sealed class ValidationResult
    {
        private ValidationResult(NormalizedEvaluateOrderRequest? value, McpToolError? error)
        {
            Value = value;
            Error = error;
        }

        public NormalizedEvaluateOrderRequest? Value { get; }

        public McpToolError? Error { get; }

        public static ValidationResult Ok(NormalizedEvaluateOrderRequest value)
        {
            return new ValidationResult(value, null);
        }

        public static ValidationResult Fail(McpToolError error)
        {
            return new ValidationResult(null, error);
        }
    }
}
