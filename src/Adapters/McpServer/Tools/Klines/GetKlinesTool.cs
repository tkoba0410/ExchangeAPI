using System.Globalization;
using System.Text.RegularExpressions;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;
using BinanceGetKlines = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlines;
using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;

namespace ExchangeApi.Adapters.McpServer.Tools.Klines;

public sealed class GetKlinesTool
{
    private const string SupportedVenue = "binance";
    private static readonly Regex Rfc3339TimestampPattern = new(
        @"^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d{1,7})?(?:Z|[+-]\d{2}:\d{2})$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly IBinanceKlinesGateway _gateway;

    public GetKlinesTool(IBinanceKlinesGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<McpToolExecutionResult<GetKlinesResponse>> ExecuteAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default)
    {
        var validation = Validate(request);
        if (validation.Error is not null)
        {
            return McpToolExecutionResult<GetKlinesResponse>.Failure(validation.Error);
        }

        var normalized = validation.Value!;
        var nativeRequest = new BinanceGetKlinesRequest
        {
            Symbol = normalized.Symbol,
            Interval = normalized.Interval,
            StartTime = normalized.StartTime,
            EndTime = normalized.EndTime,
            TimeZone = null,
            Limit = normalized.Limit,
        };

        var call = await _gateway.GetKlinesAsync(nativeRequest, cancellationToken);
        if (!call.IsSuccess || call.Response is null)
        {
            return McpToolExecutionResult<GetKlinesResponse>.Failure(
                UpstreamError(
                    errorCode: "market_unavailable",
                    message: "Failed to load klines from upstream.",
                    endpoint: "GetKlines",
                    symbol: normalized.Symbol,
                    error: call.Error));
        }

        var response = new GetKlinesResponse
        {
            Venue = SupportedVenue,
            Symbol = normalized.Symbol,
            Interval = BinanceApiStringEnum<BinanceInterval>.Format(normalized.Interval),
            Candles = call.Response.Select(MapCandle).ToArray(),
        };

        return McpToolExecutionResult<GetKlinesResponse>.Success(response);
    }

    private static ValidationResult Validate(GetKlinesRequest request)
    {
        var venue = request.Venue.Trim().ToLowerInvariant();
        if (!string.Equals(venue, SupportedVenue, StringComparison.Ordinal))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_venue",
                    message: "Venue must be binance.",
                    details: new Dictionary<string, string?> { ["venue"] = request.Venue }));
        }

        var symbol = request.Symbol.Trim().ToUpperInvariant();
        if (!BinanceKlineSymbolSet.Contains(symbol))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_symbol",
                    message: "Unsupported symbol.",
                    details: new Dictionary<string, string?> { ["symbol"] = symbol }));
        }

        var intervalText = request.Interval.Trim();
        if (!BinanceApiStringEnum<BinanceInterval>.TryParse(intervalText, out var interval))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_interval",
                    message: "Unsupported interval.",
                    details: new Dictionary<string, string?> { ["interval"] = request.Interval }));
        }

        if (request.Limit is int limit && (limit < 1 || limit > 1000))
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_limit",
                    message: "Limit must be between 1 and 1000.",
                    details: new Dictionary<string, string?> { ["limit"] = request.Limit.Value.ToString(CultureInfo.InvariantCulture) }));
        }

        if (!TryParseTimestamp(request.StartTime, out var startTime, out var startError))
        {
            return ValidationResult.Fail(startError!);
        }

        if (!TryParseTimestamp(request.EndTime, out var endTime, out var endError))
        {
            return ValidationResult.Fail(endError!);
        }

        if (startTime.HasValue && endTime.HasValue && startTime.Value > endTime.Value)
        {
            return ValidationResult.Fail(
                ValidationError(
                    errorCode: "invalid_time_range",
                    message: "startTime must be earlier than or equal to endTime.",
                    details: new Dictionary<string, string?>
                    {
                        ["startTime"] = request.StartTime,
                        ["endTime"] = request.EndTime,
                    }));
        }

        return ValidationResult.Ok(
            new NormalizedRequest(
                venue,
                symbol,
                interval,
                startTime?.ToUnixTimeMilliseconds(),
                endTime?.ToUnixTimeMilliseconds(),
                request.Limit));
    }

    private static bool TryParseTimestamp(
        string? value,
        out DateTimeOffset? timestamp,
        out McpToolError? error)
    {
        if (value is null)
        {
            timestamp = null;
            error = null;
            return true;
        }

        if (!Rfc3339TimestampPattern.IsMatch(value) ||
            !DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            timestamp = null;
            error = ValidationError(
                errorCode: "invalid_time_range",
                message: "Timestamp must be an RFC 3339 string with explicit Z or numeric offset.",
                details: new Dictionary<string, string?> { ["timestamp"] = value });
            return false;
        }

        timestamp = parsed.ToUniversalTime();
        error = null;
        return true;
    }

    private static KlineCandle MapCandle(BinanceGetKlines.Item item)
    {
        return new KlineCandle
        {
            OpenTime = FormatTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(item.OpenTime)),
            CloseTime = FormatTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(item.CloseTime)),
            Open = FormatDecimal(item.OpenPrice),
            High = FormatDecimal(item.HighPrice),
            Low = FormatDecimal(item.LowPrice),
            Close = FormatDecimal(item.ClosePrice),
            Volume = FormatDecimal(item.Volume),
            QuoteVolume = FormatDecimal(item.QuoteAssetVolume),
            TradeCount = item.NumberOfTrades,
            TakerBuyBaseVolume = FormatDecimal(item.TakerBuyBaseAssetVolume),
            TakerBuyQuoteVolume = FormatDecimal(item.TakerBuyQuoteAssetVolume),
        };
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string FormatTimestamp(DateTimeOffset value)
    {
        var utc = value.ToUniversalTime();
        var baseText = utc.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
        var fractionalTicks = utc.Ticks % TimeSpan.TicksPerSecond;
        if (fractionalTicks == 0)
        {
            return $"{baseText}Z";
        }

        var fraction = fractionalTicks.ToString("D7", CultureInfo.InvariantCulture).TrimEnd('0');
        return $"{baseText}.{fraction}Z";
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

    private sealed record NormalizedRequest(
        string Venue,
        string Symbol,
        BinanceInterval Interval,
        long? StartTime,
        long? EndTime,
        int? Limit);

    private sealed class ValidationResult
    {
        private ValidationResult(NormalizedRequest? value, McpToolError? error)
        {
            Value = value;
            Error = error;
        }

        public NormalizedRequest? Value { get; }

        public McpToolError? Error { get; }

        public static ValidationResult Ok(NormalizedRequest value)
        {
            return new ValidationResult(value, null);
        }

        public static ValidationResult Fail(McpToolError error)
        {
            return new ValidationResult(null, error);
        }
    }
}
