using System;
using System.Net.Http;
using Exchange.Bittrade.Abstract.Adapters;
using Exchange.Bittrade.Abstract.Apis;
using Exchange.Bittrade.Abstract.Apis.ExchangeInfo;
using Exchange.Bittrade.Abstract.Facade;
using Exchange.Bittrade.Raw;
using Common.Interfaces;
using Core.Transport.Observability;
using Core.Transport.Policy;
using Core.Transport.Protocol;
using Core.Transport.Http;
namespace Exchange.Bittrade.Abstract.Factory;

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
