using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Market;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Call;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Api.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Composition.Bootstrap.Factories.BittradeFactory. This factory will be removed in a future major release.")]
public static class BittradeClientFactory
{
    private static readonly Uri BaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IMarketDataApi CreatePublic() => CreatePublicClient();

    public static BittradePublicClient CreatePublicClient(
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null) =>
        new BittradePublicClient(BittradeApiBundle.FromRestClient(
            CreateRestClient(observer: observer, logger: logger)));

    public static IExchangeInfoApi CreateExchangeInfo() =>
        new BittradeExchangeInfoApi(BittradeNormalizeFactory.FromRestClient(CreateRestClient()).ExchangeInfo);

    public static (IMarketDataApi Market, ITradingApi Trading, IAccountApi Account, IExchangeInfoApi ExchangeInfo) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient, accountId);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var normalizedMarkets = new BittradeNormalizedMarketResolver(markets);
        var tradingApi = new BittradeNormalizedTradingApi(normalizeBundle.Raw, normalizedMarkets, accountId);
        var trading = new BittradeTradingApi(tradingApi);
        IAccountApi account = new BittradeAccountApi(normalizeBundle.Account);
        return (new MarketApi(normalizeBundle.MarketData, markets), trading, account, exchangeInfo);
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
