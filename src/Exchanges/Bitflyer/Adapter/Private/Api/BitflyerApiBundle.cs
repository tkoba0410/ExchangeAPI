using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Adapter.ExchangeInfo.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.ExchangeInfo.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Transport.Wire;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

/// <summary>
/// bitFlyer API 実装のセットをまとめるバンドル。
/// テスト向けにモック実装を差し替えやすくする。
/// </summary>
internal sealed class BitflyerApiBundle
{
    public BitflyerNormalizedPublicApi Public { get; }
    public IBitflyerNormalizedApi Normalized { get; }
    public BitflyerExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }

    public BitflyerApiBundle(
        IBitflyerNormalizedApi normalized,
        BitflyerNormalizedPublicApi publicApi,
        BitflyerExchangeInfoApi exchangeInfo,
        IExchangeMarketResolver markets)
    {
        Normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public static BitflyerApiBundle FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var wire = new WireTransport(restClient);
        var raw = new BitflyerRawApi(wire);
        var publicApi = new BitflyerNormalizedPublicApi(raw);
        var exchangeInfo = new BitflyerExchangeInfoApi(publicApi);
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(raw, markets);
        return new BitflyerApiBundle(normalized, publicApi, exchangeInfo, contractMarkets);
    }
}
