using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Facade;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public BitflyerNormalizedMarketDataFacade MarketData { get; }
    public IBitflyerNormalizedAccountApi Account { get; }
    public IBitflyerNormalizedMarginApi Margin { get; }
    public IBitflyerNormalizedTradingApi Trading { get; }
    public object? RawBundle { get; }

    public BitflyerApiBundle(
        BitflyerNormalizedMarketDataFacade marketData,
        IBitflyerNormalizedAccountApi account,
        IBitflyerNormalizedMarginApi margin,
        IBitflyerNormalizedTradingApi trading,
        object? rawBundle = null)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Margin = margin ?? throw new ArgumentNullException(nameof(margin));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        RawBundle = rawBundle;
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var normalized = BitflyerNormalizedApi.FromRestClient(restClient);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var privateApi = BitflyerNormalizeFactory.CreateAccountApi(restClient, markets);
        var marginApi = BitflyerNormalizeFactory.CreateMarginApi(restClient, markets);
        var tradingApi = BitflyerNormalizeFactory.CreateTradingApi(restClient, markets);
        return new BitflyerApiBundle(
            marketData: normalized.MarketData,
            account: privateApi,
            margin: marginApi,
            trading: tradingApi,
            rawBundle: null);
    }
}
