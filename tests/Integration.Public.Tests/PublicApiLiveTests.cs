using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Composition;
using ExchangeApi.Exchanges.Bittrade.Composition;
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
        var client = BitflyerFactory.CreateClient(new BitflyerFactoryOptions
        {
            Observer = _observer,
        });
        Assert.NotNull(client.Public);
        var publicApi = client.Public!;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await publicApi.GetTickerAsync(new Symbol("BTC/JPY"), cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.TickerResponse>.Ok>(call.Result);
        var ticker = ok.Response;
        var msg = $"bitFlyer Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.True(ticker.LastTradedPrice.Value > 0);
    }

    [LiveFact]
    public async Task Bitflyer_PublicExchangeInfo_Works()
    {
        var client = BitflyerFactory.CreateClient(new BitflyerFactoryOptions
        {
            Observer = _observer,
        });
        Assert.NotNull(client.Public);
        var publicApi = client.Public!;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await publicApi.GetExchangeInfoAsync(cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse>.Ok>(call.Result);
        var info = ok.Response;
        var msg = $"bitFlyer ExchangeInfo markets={info.Markets.Count}";
        Log(msg);

        Assert.NotEmpty(info.Markets);
    }

    [LiveFact]
    public async Task Bittrade_PublicTicker_Works()
    {
        var client = BittradeFactory.CreateClient(new BittradeFactoryOptions
        {
            Observer = _observer,
        });
        Assert.NotNull(client.Public);
        var publicApi = client.Public!;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await publicApi.GetTickerAsync(new Symbol("BTC/JPY"), cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.TickerResponse>.Ok>(call.Result);
        var ticker = ok.Response;
        var msg = $"Bittrade Ticker: {ticker.Symbol} last={ticker.LastTradedPrice} ts={ticker.Timestamp:o}";
        Log(msg);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.True(ticker.LastTradedPrice.Value > 0);
    }

    [LiveFact]
    public async Task Bittrade_PublicExchangeInfo_Works()
    {
        var client = BittradeFactory.CreateClient(new BittradeFactoryOptions
        {
            Observer = _observer,
        });
        Assert.NotNull(client.Public);
        var publicApi = client.Public!;
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        var call = await publicApi.GetExchangeInfoAsync(cts.Token);
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse>.Ok>(call.Result);
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
}
