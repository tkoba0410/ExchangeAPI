using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.ExchangeInfo.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.ExchangeInfo.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Composition.Bootstrap.Factories.BittradeFactory. This factory will be removed in a future major release.")]
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
        var raw = new BittradeRawApi(new ExchangeApi.Transport.Wire.WireTransport(restClient));
        var normalizedExchangeInfo = new BittradeNormalizedPublicApi(raw);
        return new BittradeExchangeInfoApi(normalizedExchangeInfo);
    }

    internal static (MarketApi Market, BittradeTradingApi Trading, BittradeAccountApi Account, BittradeExchangeInfoApi ExchangeInfo) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var raw = new BittradeRawApi(new ExchangeApi.Transport.Wire.WireTransport(restClient));
        var normalizedPublic = new BittradeNormalizedPublicApi(raw);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizedPublic);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var normalizedMarkets = new BittradeNormalizedMarketResolver(markets);
        var normalizedPrivate = new BittradeNormalizedPrivateApi(raw, normalizedMarkets, accountId);
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
