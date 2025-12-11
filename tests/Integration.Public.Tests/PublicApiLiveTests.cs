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
        Console.WriteLine($"bitFlyer Ticker: {ticker.Symbol} bid={ticker.BestBid} ask={ticker.BestAsk} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}");

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
        Console.WriteLine($"bitFlyer ExchangeInfo markets={info.Markets.Count}");

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);
        Console.WriteLine($"Bittrade Ticker: {ticker.Symbol} bid={ticker.BestBid} ask={ticker.BestAsk} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}");

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
        Console.WriteLine($"Bittrade ExchangeInfo markets={info.Markets.Count}");

        Assert.NotEmpty(info.Markets);
    }
}
