using System;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthAsync_ReturnsRawHealth()
    {
        var rawTicker = new Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var marketData = new BitflyerNormalizedMarketDataFacade(publicApi);

        var call = await marketData.GetHealthCallAsync("BTC_JPY");
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.BitflyerHealthNormalized>.Ok>(call.Result);
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
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos.BitflyerBoardStateNormalized>.Ok>(call.Result);
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
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.Market.Ticker>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }
}
