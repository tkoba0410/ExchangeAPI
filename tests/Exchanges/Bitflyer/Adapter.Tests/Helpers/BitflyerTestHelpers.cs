using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api.Markets;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;

internal static class BitflyerTestHelpers
{
    public static INormalizedApi CreateNormalizedApi(IRawApi raw, IMarketResolver markets) =>
        NormalizedApi.FromRaw(raw, markets);

    public static INormalizedApi CreateNormalizedApi(
        RawPublicDtos.GetTickerResponse ticker,
        IMarketResolver markets,
        RawPublicDtos.GetBoardResponse? board = null,
        FakeBitflyerPrivateApi? privateApi = null,
        FakeBitflyerPrivateTradingApi? tradingApi = null)
    {
        var raw = new FakeBitflyerPublicApi(ticker, board, privateApi, tradingApi);
        return NormalizedApi.FromRaw(raw, markets);
    }

    public static INormalizedApi CreateTradingApi(
        FakeBitflyerPrivateTradingApi tradingApi,
        IMarketResolver markets,
        FakeBitflyerPrivateApi? privateApi = null) =>
        CreateNormalizedApi(
            new RawPublicDtos.GetTickerResponse { ProductCode = "BTC_JPY" },
            markets,
            privateApi: privateApi,
            tradingApi: tradingApi);

    public static IMarketResolver CreateResolver()
    {
        return new NormalizedMarketResolver(new BitflyerMarketCatalogResolver());
    }
}
