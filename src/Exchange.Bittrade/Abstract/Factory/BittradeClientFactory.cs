using System;
using System.Net.Http;
using ExchangeApi.Adapter.Bittrade.Adapters;
using ExchangeApi.Adapter.Bittrade.Apis;
using ExchangeApi.Adapter.Bittrade.Apis.ExchangeInfo;
using ExchangeApi.Adapter.Bittrade.Facade;
using ExchangeApi.Adapter.Bittrade.Http;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Transport.Logging;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Transport;

namespace ExchangeApi.Adapter.Bittrade.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
public static class BittradeClientFactory
{
    private static readonly Uri BaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IMarketDataApi CreatePublic() => CreatePublicClient();

    public static BittradePublicClient CreatePublicClient(
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null) =>
        new BittradePublicClient(CreateRestClient(observer: observer, logger: logger));

    public static IExchangeInfoApi CreateExchangeInfo() =>
        new BittradeExchangeInfoApi(CreateRestClient());

    public static (IMarketDataApi Market, ITradingApi Trading, IAccountApi Account, IExchangeInfoApi ExchangeInfo, BittradeRawApiClient BittradeRaw) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var publicApi = new BittradePublicApi(restClient);
        var privateApi = new BittradePrivateApi(restClient);
        var privateTrading = new BittradePrivateTradingApi(restClient);
        var trading = new BittradeTradingApi(restClient, accountId);
        var raw = new BittradeRawApiClient(publicApi, privateApi, privateTrading);
        return (new BittradeMarketDataApi(restClient), trading, trading, new BittradeExchangeInfoApi(restClient), raw);
    }

    public static BittradeExchangeClient CreateDefault(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var market = new BittradeMarketDataApi(restClient);
        var trading = new BittradeTradingApi(restClient, accountId);
        var exchangeInfo = new BittradeExchangeInfoApi(restClient);
        var raw = new BittradeRawApiClient(
            new BittradePublicApi(restClient),
            new BittradePrivateApi(restClient),
            new BittradePrivateTradingApi(restClient));
        return new BittradeExchangeClient(market, trading, trading, exchangeInfo, raw);
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
            errorClassifier: new BittradeErrorClassifier(),
            requestSigner: signer,
            observer: observer,
            logger: logger);
        return restClient;
    }
}
