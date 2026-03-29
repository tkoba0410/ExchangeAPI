using System.Globalization;
using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Market;

public sealed class GetMarketSnapshotTool
{
    private readonly IBitflyerMarketSnapshotGateway _gateway;

    public GetMarketSnapshotTool(IBitflyerMarketSnapshotGateway gateway)
    {
        _gateway = gateway;
    }

    public async Task<McpToolExecutionResult<GetMarketSnapshotResponse>> ExecuteAsync(
        GetMarketSnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var symbol = NormalizeSymbol(request.Symbol);
        if (!BitflyerMarketRuleRegistry.TryGet(symbol, out var rule) || rule is null)
        {
            return McpToolExecutionResult<GetMarketSnapshotResponse>.Failure(
                ValidationError(
                    errorCode: "invalid_symbol",
                    message: "Unsupported symbol.",
                    details: new Dictionary<string, string?> { ["symbol"] = symbol }));
        }

        var tickerCall = await _gateway.GetTickerCallAsync(symbol, cancellationToken);
        if (!tickerCall.IsSuccess || tickerCall.Response is null)
        {
            return McpToolExecutionResult<GetMarketSnapshotResponse>.Failure(
                UpstreamError(
                    errorCode: "market_unavailable",
                    message: "Failed to load ticker from upstream.",
                    endpoint: "GetTicker",
                    symbol: symbol,
                    error: tickerCall.Error));
        }

        var boardStateCall = await _gateway.GetBoardStateCallAsync(symbol, cancellationToken);
        if (!boardStateCall.IsSuccess || boardStateCall.Response is null)
        {
            return McpToolExecutionResult<GetMarketSnapshotResponse>.Failure(
                UpstreamError(
                    errorCode: "market_unavailable",
                    message: "Failed to load board state from upstream.",
                    endpoint: "GetBoardState",
                    symbol: symbol,
                    error: boardStateCall.Error));
        }

        var response = new GetMarketSnapshotResponse
        {
            Symbol = symbol,
            Bid = FormatDecimal(tickerCall.Response.BestBid),
            Ask = FormatDecimal(tickerCall.Response.BestAsk),
            Last = FormatDecimal(tickerCall.Response.Ltp),
            Timestamp = FormatTimestamp(tickerCall.Response.Timestamp),
            Rules = new MarketSnapshotRules
            {
                MinSize = rule.MinSize,
                SizeStep = rule.SizeStep,
                PriceStep = rule.PriceStep,
            },
            Status = BitflyerMarketStatusMapper.Map(boardStateCall.Response.State, boardStateCall.Response.Health),
        };

        return McpToolExecutionResult<GetMarketSnapshotResponse>.Success(response);
    }

    private static string NormalizeSymbol(string symbol)
    {
        return symbol.Trim().ToUpperInvariant();
    }

    private static string FormatDecimal(decimal value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    private static string FormatTimestamp(DateTimeOffset value)
    {
        var utc = value.ToUniversalTime();
        var format = utc.Ticks % TimeSpan.TicksPerSecond == 0
            ? "yyyy-MM-dd'T'HH:mm:ss'Z'"
            : "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";
        return utc.ToString(format, CultureInfo.InvariantCulture);
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
}
