using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools.Market;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class ListMarketsToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsVisibleMarketCapabilities()
    {
        var tool = new ListMarketsTool(
            hasMarketSnapshot: true,
            hasKlines: true,
            hasEvaluateOrder: true);

        var result = await tool.ExecuteAsync(new ListMarketsRequest());

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<ListMarketsResponse>(result.Response);

        var btcJpy = Assert.Single(response.Markets, x => x.Venue == "bitflyer" && x.Symbol == "BTC_JPY");
        Assert.Equal(["get_market_snapshot", "evaluate_order"], btcJpy.Capabilities);

        var fxBtcJpy = Assert.Single(response.Markets, x => x.Venue == "bitflyer" && x.Symbol == "FX_BTC_JPY");
        Assert.Equal(["get_market_snapshot"], fxBtcJpy.Capabilities);

        var btcUsdt = Assert.Single(response.Markets, x => x.Venue == "binance" && x.Symbol == "BTCUSDT");
        Assert.Equal(["get_klines"], btcUsdt.Capabilities);
    }

    [Fact]
    public async Task ExecuteAsync_OmitsCapabilitiesThatAreNotVisible()
    {
        var tool = new ListMarketsTool(
            hasMarketSnapshot: true,
            hasKlines: false,
            hasEvaluateOrder: false);

        var result = await tool.ExecuteAsync(new ListMarketsRequest());

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<ListMarketsResponse>(result.Response);
        Assert.All(response.Markets.Where(x => x.Venue == "bitflyer"), market => Assert.Equal(["get_market_snapshot"], market.Capabilities));
        Assert.DoesNotContain(response.Markets, x => x.Venue == "binance");
    }
}
