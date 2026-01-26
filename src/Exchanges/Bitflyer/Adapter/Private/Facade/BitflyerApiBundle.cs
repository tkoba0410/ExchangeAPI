using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.ExchangeInfo.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Facade;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public IBitflyerNormalizedApi Normalized { get; }

    public BitflyerApiBundle(IBitflyerNormalizedApi normalized)
    {
        Normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizeFactory.FromRestClient(restClient, markets);
        return new BitflyerApiBundle(normalized);
    }
}
