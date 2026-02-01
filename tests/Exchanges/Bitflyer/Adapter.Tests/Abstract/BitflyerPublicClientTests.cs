using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthCallAsync_ReturnsRawHealth()
    {
        var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetHealthCallAsync("BTC_JPY");
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.BitflyerHealthNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Status);
    }

    [Fact]
    public async Task GetBoardStateCallAsync_ReturnsRawBoardState()
    {
        var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetBoardStateCallAsync("BTC_JPY");
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.BitflyerBoardStateNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Health);
        Assert.Equal("RUNNING", result.State);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetTickerCallAsync_UnknownSymbol_Throws()
    {
        var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);
        var client = new BitflyerPublicClient(normalized, exchangeInfo);

        var call = await client.GetTickerCallAsync(new Symbol("DOGE/JPY"));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.Market.Ticker>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }
}
