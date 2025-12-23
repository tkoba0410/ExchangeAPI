using System;
using System.Net.Http;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Transport.Observability;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Time;
using ExchangeApi.Core.Transport.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Account;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Margin;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Market;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Apis.Trading;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using ExchangeApi.Exchanges.Bitflyer.Wire.Private;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;

/// <summary>
    /// Factory for constructing bitFlyer client instances.
    /// HttpClient -> HttpTransport -> RestClient(署名/ポリシー/ログ) -> BitflyerPublicApi/BitflyerPrivateApi -> BitflyerExchangeClient.
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

        var raw = new Raw.BitflyerRawApi(restClient);
        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var wire = new BitflyerWireApi(
            marketData: publicApi,
            trading: new BitflyerWireTradingApiNotSupported(),
            account: new BitflyerWireAccountApiNotSupported(),
            exchangeInfo: publicApi);
        return new BitflyerPublicClient(publicApi, rawBundle: raw, wireBundle: wire);
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

        var raw = new Raw.BitflyerRawApi(restClient);
        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var privateApi = new BitflyerPrivateApi(restClient);
        var privateTradingApi = new BitflyerPrivateTradingApi(restClient);
        var wire = new BitflyerWireApi(raw, restClient);

        return new BitflyerExchangeClient(
            marketData: publicApi,
            account: privateApi,
            trading: privateTradingApi,
            exchangeInfo: publicApi,
            rawBundle: raw,
            wireBundle: wire);
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
