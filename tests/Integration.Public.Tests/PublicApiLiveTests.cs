using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Factory;
using ExchangeApi.Adapter.Bittrade.Factory;
using Xunit.Abstractions;

namespace Integration.Public.Tests;

public class PublicApiLiveTests
{
    private readonly ITestOutputHelper _output;

    public PublicApiLiveTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [LiveFact]
    public async Task Bitflyer_PublicTicker_Works()
    {
        var client = BitflyerClientFactory.CreatePublic();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);
        var msg = $"bitFlyer Ticker: {ticker.Symbol} bid={ticker.BestBid} ask={ticker.BestAsk} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        _output.WriteLine(msg);
        Console.WriteLine(msg);

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
        var msg = $"bitFlyer ExchangeInfo markets={info.Markets.Count}";
        _output.WriteLine(msg);
        Console.WriteLine(msg);

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);
        var msg = $"Bittrade Ticker: {ticker.Symbol} bid={ticker.BestBid} ask={ticker.BestAsk} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        _output.WriteLine(msg);
        Console.WriteLine(msg);

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
        var msg = $"Bittrade ExchangeInfo markets={info.Markets.Count}";
        _output.WriteLine(msg);
        Console.WriteLine(msg);

        Assert.NotEmpty(info.Markets);
    }
}
