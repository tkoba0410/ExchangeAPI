using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Core.Transport.Http;
using ExchangeApi.Core.Transport.Protocol;
using System.Net.Http;
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

        var ticker = await client.GetTickerAsync(new Symbol("BTC/JPY"), cts.Token);
        var msg = $"bitFlyer Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.True(ticker.LastTradedPrice.Value > 0);
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
        var client = CreateBittradePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var ticker = await client.GetTickerAsync(new Symbol("BTC/JPY"), cts.Token);
        var msg = $"Bittrade Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.True(ticker.LastTradedPrice.Value > 0);
    }

    [LiveFact]
    public async Task Bittrade_PublicExchangeInfo_Works()
    {
        var client = CreateBittradePublicClient();
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

    private BittradePublicClient CreateBittradePublicClient()
    {
        var baseUri = new Uri("https://api-cloud.bittrade.co.jp/");
        var http = new HttpClient { BaseAddress = baseUri };
        var transport = new HttpTransport(http, disposeHttpClient: true);
        var restClient = new RestClient(
            baseUri,
            transport,
            observer: _observer,
            errorClassifier: new BittradeErrorClassifier());
        return new BittradePublicClient(restClient);
    }
}
