using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Factory;
using ExchangeApi.Adapter.Bittrade.Factory;

namespace Integration.Public.Tests;

public class PublicApiLiveTests
{
    [LiveFact]
    public async Task Bitflyer_PublicTicker_Works()
    {
        var client = BitflyerClientFactory.CreatePublic();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);

        Assert.Equal("BTC/JPY", ticker.Symbol);
        Assert.True(ticker.BestBid > 0);
        Assert.True(ticker.BestAsk > 0);
    }

    [LiveFact]
    public async Task Bitflyer_PublicExchangeInfo_Works()
    {
        var client = BitflyerClientFactory.CreatePublic();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var info = await client.GetExchangeInfoAsync(cts.Token);

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);

        Assert.Equal("BTC/JPY", ticker.Symbol);
        Assert.True(ticker.BestBid > 0);
        Assert.True(ticker.BestAsk > 0);
    }

    [LiveFact]
    public async Task Bittrade_PublicExchangeInfo_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var info = await client.GetExchangeInfoAsync(cts.Token);

        Assert.NotEmpty(info.Markets);
    }
}
