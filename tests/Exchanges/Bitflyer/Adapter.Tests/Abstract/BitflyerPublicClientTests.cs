using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthCallAsync_ReturnsRawHealth()
    {
        var rawTicker = new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = NormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetHealthCallAsync(ProductCode.ParseOrThrow("BTC_JPY"));
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.HealthNormalized>.Ok>(call.Result);
        var result = ok.Response;

        Assert.Equal("NORMAL", result.Status?.Value);
    }

    [Fact]
    public async Task GetBoardStateCallAsync_ReturnsRawBoardState()
    {
        var rawTicker = new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalized = NormalizedApi.FromRaw(publicApi, markets);

        var call = await normalized.GetBoardStateCallAsync(ProductCode.ParseOrThrow("BTC_JPY"));
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos.BoardStateNormalized>.Ok>(call.Result);
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
        var markets = new NormalizedMarketResolver(contractMarkets);
        var normalized = NormalizedApi.FromRaw(publicApi, markets);
        var client = new PublicClient(normalized, exchangeInfo);

        var call = await client.GetTickerAsync(new TickerRequest(new Symbol("DOGE/JPY")));
        var err = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.TickerResponse>.Err>(call.Result);
        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
    }
}
