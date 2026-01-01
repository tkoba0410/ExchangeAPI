using System;
using System.Net.Http;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.Account;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Boundary.Adapters.Common.NotSupported;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Common.Enums;
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
        new BittradeExchangeInfoApi(BittradeNormalizeFactory.FromRestClient(CreateRestClient()).ExchangeInfo);

    public static (IMarketDataApi Market, ITradingApi Trading, IAccountApi Account, IExchangeInfoApi ExchangeInfo, object? RawBundle) CreatePrivate(
        string accessKey,
        string secretKey,
        string accountId)
    {
        var restClient = CreateRestClient(new BittradeRequestSigner(accessKey, secretKey));
        var normalizeBundle = BittradeNormalizeFactory.FromRestClient(restClient, accountId);
        var exchangeInfo = new BittradeExchangeInfoApi(normalizeBundle.ExchangeInfo);
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var tradingApi = BittradeNormalizeFactory.CreateTradingApi(restClient, markets, accountId);
        var trading = new BittradeTradingApi(tradingApi);
        IAccountApi account = normalizeBundle.Account is null
            ? new NotSupportedAccountApi(ExchangeCode.Bittrade)
            : new BittradeAccountApi(normalizeBundle.Account);
        return (new BittradeMarketDataApi(normalizeBundle.MarketData, markets), trading, account, exchangeInfo, normalizeBundle.RawBundle);
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
