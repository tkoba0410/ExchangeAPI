using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public BitflyerNormalizedMarketDataFacade MarketData { get; }
    public IBitflyerNormalizedAccountApi Account { get; }
    public IBitflyerNormalizedTradingApi Trading { get; }

    public BitflyerApiBundle(
        BitflyerNormalizedMarketDataFacade marketData,
        IBitflyerNormalizedAccountApi account,
        IBitflyerNormalizedTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizeFactory.FromRestClient(restClient, markets);
        return new BitflyerApiBundle(
            marketData: normalized.MarketData,
            account: normalized.Account,
            trading: normalized.Trading);
    }
}
