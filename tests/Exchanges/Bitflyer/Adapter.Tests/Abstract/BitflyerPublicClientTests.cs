using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthCallAsync_ReturnsRawHealth()
    {
        var rawTicker = new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetHealthCallAsync(ProductCode.ParseOrThrow("BTC_JPY"));
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos.BitflyerHealthNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Status?.Value);
    }

    [Fact]
    public async Task GetBoardStateCallAsync_ReturnsRawBoardState()
    {
        var rawTicker = new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetBoardStateCallAsync(ProductCode.ParseOrThrow("BTC_JPY"));
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos.BitflyerBoardStateNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Health?.Value);
        Assert.Equal("RUNNING", result.State?.Value);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetTickerAsync_UnknownSymbol_Throws()
    {
        var rawTicker = new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(publicApi, markets);
        var client = new BitflyerPublicClient(normalized, exchangeInfo);

        var call = await client.GetTickerAsync(new Symbol("DOGE/JPY"));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.TickerResponse>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }
}
