using System;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using ExchangeApi.Adapter.Bittrade.Factory;
using Xunit.Abstractions;

namespace Integration.Public.Tests;

public class PublicApiLiveTests
{
    private readonly ITestOutputHelper _output;
    private readonly PublicApiLoggingObserver _observer;

    public PublicApiLiveTests(ITestOutputHelper output)
    {
        _output = output;
        _observer = new PublicApiLoggingObserver(Log);
    }

    [LiveFact]
    public async Task Bitflyer_PublicTicker_Works()
    {
        var options = new BitflyerClientOptions { Observer = _observer };
        var client = BitflyerClientFactory.CreatePublic(options);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);
        var msg = $"bitFlyer Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal("BTC/JPY", ticker.Symbol);
        Assert.True(ticker.LastTradedPrice > 0);
    }

    [LiveFact]
    public async Task Bitflyer_PublicExchangeInfo_Works()
    {
        var options = new BitflyerClientOptions { Observer = _observer };
        var client = BitflyerClientFactory.CreatePublic(options);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var info = await client.GetExchangeInfoAsync(cts.Token);
        var msg = $"bitFlyer ExchangeInfo markets={info.Markets.Count}";
        Log(msg);

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient(observer: _observer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync("BTC/JPY", cts.Token);
        var msg = $"Bittrade Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal("BTC/JPY", ticker.Symbol);
        Assert.True(ticker.LastTradedPrice > 0);
    }

    [LiveFact]
    public async Task Bittrade_PublicExchangeInfo_Works()
    {
        var client = BittradeClientFactory.CreatePublicClient(observer: _observer);
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var info = await client.GetExchangeInfoAsync(cts.Token);
        var msg = $"Bittrade ExchangeInfo markets={info.Markets.Count}";
        Log(msg);

        Assert.NotEmpty(info.Markets);
    }

    private void Log(string message)
    {
        _output.WriteLine(message);
        Console.WriteLine(message);
    }
}
