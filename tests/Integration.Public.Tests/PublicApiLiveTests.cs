using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Protocol;
using System.Net.Http;
using Xunit.Abstractions;

namespace ExchangeApi.Tests.Integration.Public.Tests;

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

        var call = await client.GetTickerCallAsync(new Symbol("BTC/JPY"), cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.Ticker>.Ok>(call.Result);
        var ticker = ok.Response;
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

        var call = await client.GetExchangeInfoCallAsync(cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;
        var msg = $"bitFlyer ExchangeInfo markets={info.Markets.Count}";
        Log(msg);

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = CreateBittradePublicClient();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await client.GetTickerCallAsync(new Symbol("BTC/JPY"), cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.Ticker>.Ok>(call.Result);
        var ticker = ok.Response;
        var msg = $"Bittrade Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.True(ticker.LastTradedPrice.Value > 0);
    }

    [LiveFact]
    public async Task Bittrade_PublicExchangeInfo_Works()
    {
        #pragma warning disable CS0618
        var infoApi = ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Factory.BittradeClientFactory.CreateExchangeInfo();
        #pragma warning restore CS0618
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await infoApi.GetExchangeInfoCallAsync(cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;
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
            observer: _observer);
        return new BittradePublicClient(restClient);
    }
}
