using System;
using System.Net.Http;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Observability;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Time;
using ExchangeApi.Core.Transport.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Normalize;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Call;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Factory;

/// <summary>
/// Factory for constructing bitFlyer client instances.
/// HttpClient -> HttpTransport -> RestClient(署名/ポリシー/ログ) -> Raw/Private API -> BitflyerExchangeClient.
/// </summary>
public static class BitflyerClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// Public API のみを利用する軽量クライアントを作成する。
    /// 署名を行わず、マーケット/ExchangeInfo 取得に限定する。
    /// </summary>
    public static BitflyerPublicClient CreatePublic(
        BitflyerClientOptions? options = null,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        options ??= new BitflyerClientOptions();

        var http = httpClient ?? options.HttpClient ?? new HttpClient { BaseAddress = BitflyerApiBaseUri };
        IHttpTransport baseTransport = transportOverride ?? new HttpTransport(http, disposeHttpClient: false);

        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(options.PolicyOptions);
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? BitflyerErrorClassifier.Instance;

        IRestClient restClient = new RestClient(
            BitflyerApiBaseUri,
            baseTransport,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier);

        var normalized = BitflyerNormalizedApi.FromRestClient(restClient);
        return new BitflyerPublicClient(normalized.MarketData, rawBundle: null);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller.
    /// </summary>
    public static BitflyerExchangeClient Create(
        string apiKey,
        string apiSecret,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null,
        HttpClient? httpClient = null)
    {
        var options = new BitflyerClientOptions
        {
            Policy = policy,
            Logger = logger,
            Observer = observer,
            ErrorClassifier = errorClassifier,
            HttpClient = httpClient,
        };

        return Create(apiKey, apiSecret, options, httpClient: httpClient);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller using options.
    /// </summary>
    public static BitflyerExchangeClient Create(
        string apiKey,
        string apiSecret,
        BitflyerClientOptions? options,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(apiSecret));
        }

        options ??= new BitflyerClientOptions();

        var http = httpClient ?? options.HttpClient ?? new HttpClient { BaseAddress = BitflyerApiBaseUri };

        IHttpTransport baseTransport = transportOverride ?? new HttpTransport(http, disposeHttpClient: false);

        IExchangeClock clock = new SystemClock();
        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(options.PolicyOptions);
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? BitflyerErrorClassifier.Instance;

        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);

        IRestClient restClient = new RestClient(
            BitflyerApiBaseUri,
            baseTransport,
            requestSigner: signer,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier);

        var normalized = BitflyerNormalizedApi.FromRestClient(restClient);
        var exchangeInfo = new BitflyerExchangeInfoApi();
        var markets = new ExchangeInfoMarketResolver(exchangeInfo);
        var accountApi = BitflyerNormalizeFactory.CreateAccountApi(restClient, markets);
        var marginApi = BitflyerNormalizeFactory.CreateMarginApi(restClient, markets);
        var tradingApi = BitflyerNormalizeFactory.CreateTradingApi(restClient, markets);

        return new BitflyerExchangeClient(
            marketData: normalized.MarketData,
            account: accountApi,
            margin: marginApi,
            trading: tradingApi,
            rawBundle: null);
    }

    /// <summary>
    /// Create bitFlyer client using a credential provider to retrieve API key/secret.
    /// </summary>
    public static BitflyerExchangeClient Create(
        IApiCredentialProvider provider,
        ExchangeCode exchange,
        string accountId,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null,
        HttpClient? httpClient = null)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var credentials = provider.Get(exchange, accountId);
        var options = new BitflyerClientOptions
        {
            Policy = policy,
            Logger = logger,
            Observer = observer,
            ErrorClassifier = errorClassifier,
            HttpClient = httpClient
        };
        return Create(credentials.ApiKey, credentials.ApiSecret, options);
    }
}
