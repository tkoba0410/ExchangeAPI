using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Wire.Internal;
using ExchangeApi.Transport.Wire;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

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
        var wireTransport = new WireTransport(restClient);
        var wire = new BitflyerWireCallExecutor(wireTransport);
        var raw = new BitflyerRawApi(wire);
        var publicApi = new BitflyerNormalizedPublicApi(raw);
        var exchangeInfo = new BitflyerExchangeInfoApi(publicApi);
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = BitflyerNormalizedApi.FromRaw(raw, markets);
        return new BitflyerApiBundle(normalized, publicApi, exchangeInfo, contractMarkets);
    }
}
