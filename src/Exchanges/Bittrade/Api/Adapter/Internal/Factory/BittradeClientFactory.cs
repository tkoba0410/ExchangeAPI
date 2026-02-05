using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Wire.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Wire;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Api.Adapter.Internal.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Exchanges.Bittrade.Composition.BittradeFactory. This factory will be removed in a future major release.")]
public static class BittradeClientFactory
{
    private static readonly Uri BaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IPublicApi CreatePublic() => CreatePublicClient();

    public static BittradePublicClient CreatePublicClient(
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null) =>
        new BittradePublicClient(BittradeApiBundle.FromRestClient(
            CreateRestClient(observer: observer, logger: logger)));

    public static BittradeExchangeInfoApi CreateExchangeInfo()
    {
        var restClient = CreateRestClient();
        var wireTransport = new WireTransport(restClient);
        var wire = new BittradeWireCallExecutor(wireTransport);
        var raw = new BittradeRawApi(wire);
        var normalizedExchangeInfo = new BittradeNormalizedPublicApi(raw);
        return new BittradeExchangeInfoApi(normalizedExchangeInfo);
    }

    internal static (MarketApi Market, BittradeTradingApi Trading, BittradeAccountApi Account, BittradeExchangeInfoApi ExchangeInfo) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var wireTransport = new WireTransport(restClient);
        var wire = new BittradeWireCallExecutor(wireTransport);
        var raw = new BittradeRawApi(wire);
        var normalizedPublic = new BittradeNormalizedPublicApi(raw);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizedPublic);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var normalizedMarkets = new BittradeNormalizedMarketResolver(markets);
        var normalizedPrivate = new BittradeNormalizedPrivateApi(raw, normalizedMarkets, new FreeText(accountId));
        var trading = new BittradeTradingApi(normalizedPrivate);
        var account = new BittradeAccountApi(normalizedPrivate);
        return (new MarketApi(normalizedPublic, markets), trading, account, exchangeInfo);
    }

    public static BittradeExchangeClient CreateDefault(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var bundle = BittradeApiBundle.FromRestClient(restClient, accountId);
        return new BittradeExchangeClient(bundle);
    }

    private static RestClient CreateRestClient(
        IRequestSigner? signer = null,
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null)
    {
        var handler = new HttpClientHandler();
        var transport = new HttpTransport(new HttpClient(handler, disposeHandler: true), disposeHttpClient: true);
        var policyObserver = NoOpPolicyObserver.Instance;
        var policy = HttpPolicyFactory.CreateDefault(observer: policyObserver);
        var restClient = new RestClient(
            BaseUri,
            transport,
            policy: policy,
            errorClassifier: BittradeErrorClassifier.Instance,
            requestSigner: signer,
            observer: observer,
            logger: logger);
        return restClient;
    }
}
