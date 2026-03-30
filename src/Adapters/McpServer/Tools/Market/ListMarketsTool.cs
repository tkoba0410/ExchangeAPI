using ExchangeApi.Adapters.McpServer.Schema;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Mapping;

namespace ExchangeApi.Adapters.McpServer.Tools.Market;

public sealed class ListMarketsTool
{
    private readonly bool _hasMarketSnapshot;
    private readonly bool _hasKlines;
    private readonly bool _hasEvaluateOrder;

    public ListMarketsTool(
        bool hasMarketSnapshot,
        bool hasKlines,
        bool hasEvaluateOrder)
    {
        _hasMarketSnapshot = hasMarketSnapshot;
        _hasKlines = hasKlines;
        _hasEvaluateOrder = hasEvaluateOrder;
    }

    public Task<McpToolExecutionResult<ListMarketsResponse>> ExecuteAsync(
        ListMarketsRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = request;
        _ = cancellationToken;

        var markets = new List<SupportedMarketDescriptor>();

        if (_hasMarketSnapshot)
        {
            foreach (var symbol in BitflyerMarketRuleRegistry.Entries.Keys.OrderBy(x => x))
            {
                var capabilities = new List<string> { "get_market_snapshot" };
                if (_hasEvaluateOrder && string.Equals(symbol, "BTC_JPY", StringComparison.Ordinal))
                {
                    capabilities.Add("evaluate_order");
                }

                markets.Add(
                    new SupportedMarketDescriptor
                    {
                        Venue = "bitflyer",
                        Symbol = symbol,
                        Capabilities = capabilities,
                    });
            }
        }

        if (_hasKlines)
        {
            foreach (var symbol in BinanceKlineSymbolSet.Entries.OrderBy(x => x))
            {
                markets.Add(
                    new SupportedMarketDescriptor
                    {
                        Venue = "binance",
                        Symbol = symbol,
                        Capabilities = ["get_klines"],
                    });
            }
        }

        return Task.FromResult(
            McpToolExecutionResult<ListMarketsResponse>.Success(
                new ListMarketsResponse
                {
                    Markets = markets,
                }));
    }
}
