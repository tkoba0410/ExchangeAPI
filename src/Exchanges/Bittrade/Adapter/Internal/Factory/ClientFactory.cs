using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Wire;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Exchanges.Bittrade.Composition.Factory. This factory will be removed in a future major release.")]
public static class ClientFactory
{
    private static readonly Uri BaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IPublicApi CreatePublic() => CreatePublicClient();

    public static PublicClient CreatePublicClient(
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null) =>
        new PublicClient(ApiBundle.FromRestClient(
            CreateRestClient(observer: observer, logger: logger)));

    public static BittradeExchangeInfoApi CreateExchangeInfo()
    {
        var restClient = CreateRestClient();
        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);
        var normalizedExchangeInfo = new NormalizedPublicApi(raw);
        return new BittradeExchangeInfoApi(normalizedExchangeInfo);
    }

    internal static (MarketApi Market, TradingApi Trading, AccountApi Account, BittradeExchangeInfoApi ExchangeInfo) CreatePrivate(
        string accessKey,
        string secretKey,
        AccountId accountId)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = accountId;
        var restClient = CreateRestClient(new RequestSigner(accessKey, secretKey));
        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);
        var normalizedPublic = new NormalizedPublicApi(raw);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizedPublic);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var normalizedMarkets = new NormalizedMarketResolver(markets);
        var normalizedPrivate = new NormalizedPrivateApi(raw, normalizedMarkets, normalizedAccountId);
        var trading = new TradingApi(normalizedPrivate);
        var account = new AccountApi(normalizedPrivate);
        return (new MarketApi(normalizedPublic, markets), trading, account, exchangeInfo);
    }

    public static ExchangeClient CreateDefault(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var normalizedAccountId = AccountId.ParseOrThrow(accountId);
        var restClient = CreateRestClient(new RequestSigner(accessKey, secretKey));
        var bundle = ApiBundle.FromRestClient(restClient, normalizedAccountId);
        return new ExchangeClient(bundle);
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
            errorClassifier: ErrorClassifier.Instance,
            requestSigner: signer,
            observer: observer,
            logger: logger);
        return restClient;
    }
}
