using System;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthAsync_ReturnsRawHealth()
    {
        var rawTicker = new Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var marketData = new BitflyerNormalizedMarketDataFacade(publicApi);

        var call = await marketData.GetHealthCallAsync("BTC_JPY");
        var ok = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos.BitflyerHealthNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Status);
    }

    [Fact]
    public async Task GetBoardStateAsync_ReturnsRawBoardState()
    {
        var rawTicker = new Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var marketData = new BitflyerNormalizedMarketDataFacade(publicApi);

        var call = await marketData.GetBoardStateCallAsync("BTC_JPY");
        var ok = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos.BitflyerBoardStateNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Health);
        Assert.Equal("RUNNING", result.State);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetTickerAsync_UnknownSymbol_Throws()
    {
        var rawTicker = new Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var marketData = new BitflyerNormalizedMarketDataFacade(publicApi);
        var client = new BitflyerPublicClient(marketData);

        var call = await client.GetTickerCallAsync(new Symbol("ETH/JPY"));
        var err = Assert.IsType<ExchangeApi.Spec.CallCommon.CallResult<ExchangeApi.Contracts.Dtos.Ticker>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }
}
