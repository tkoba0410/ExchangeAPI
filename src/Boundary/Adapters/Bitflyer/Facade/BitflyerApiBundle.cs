using System;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public IBitflyerRawAccountApi Account { get; }
    public IBitflyerRawPrivateTradingApi Trading { get; }
    public object? RawBundle { get; }

    public BitflyerApiBundle(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerRawAccountApi account,
        IBitflyerRawPrivateTradingApi trading,
        object? rawBundle = null)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        RawBundle = rawBundle;
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var raw = new Raw.BitflyerRawApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);
        var privateTradingApi = new BitflyerPrivateTradingApi(restClient);
        return new BitflyerApiBundle(
            marketData: raw.MarketData,
            account: privateApi,
            trading: privateTradingApi,
            rawBundle: raw);
    }
}
