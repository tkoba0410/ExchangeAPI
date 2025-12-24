using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Core.Transport.Observability;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Http;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Composition.Factory.BittradeFactory. This factory will be removed in a future major release.")]
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
        new BittradeExchangeInfoApi(CreateRestClient());

    public static (IMarketDataApi Market, ITradingApi Trading, IAccountApi Account, IExchangeInfoApi ExchangeInfo, BittradeRawApi BittradeRaw) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var publicApi = new BittradePublicApi(restClient);
        var privateApi = new BittradePrivateApi(restClient);
        var privateTrading = new BittradePrivateTradingApi(restClient);
        var raw = new BittradeRawApi(publicApi, privateApi, privateTrading);
        var trading = new BittradeTradingApi(new BittradeWireTradingApi(raw.Trading, accountId));
        var account = new BittradeAccountApi(restClient, accountId);
        return (new BittradeMarketDataApi(restClient), trading, account, new BittradeExchangeInfoApi(restClient), raw);
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
            errorClassifier: new BittradeErrorClassifier(),
            requestSigner: signer,
            observer: observer,
            logger: logger);
        return restClient;
    }
}
