using System;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public IBitflyerWireMarketDataApi MarketData { get; }
    public IBitflyerWireAccountApi Account { get; }
    public IBitflyerWireTradingApi Trading { get; }
    public IBitflyerWireExchangeInfoApi ExchangeInfo { get; }
    public object? RawBundle { get; }
    public object? WireBundle { get; }

    public BitflyerApiBundle(
        IBitflyerWireMarketDataApi marketData,
        IBitflyerWireAccountApi account,
        IBitflyerWireTradingApi trading,
        IBitflyerWireExchangeInfoApi exchangeInfo,
        object? rawBundle = null,
        object? wireBundle = null)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        RawBundle = rawBundle;
        WireBundle = wireBundle;
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var raw = new Raw.BitflyerRawApi(restClient);
        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var privateApi = new BitflyerPrivateApi(restClient);
        var privateTradingApi = new BitflyerPrivateTradingApi(restClient);
        var wire = new BitflyerWireApi(raw, restClient);
        return new BitflyerApiBundle(
            marketData: publicApi,
            account: privateApi,
            trading: privateTradingApi,
            exchangeInfo: publicApi,
            rawBundle: raw,
            wireBundle: wire);
    }
}
