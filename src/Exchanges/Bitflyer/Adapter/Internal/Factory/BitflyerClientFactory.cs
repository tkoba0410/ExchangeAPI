using System;
using System.Net.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Transport.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Wire;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

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

        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? BitflyerHttpPolicyDefaults.Create());
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

        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);
        var publicApi = new NormalizedPublicApi(raw);
        var exchangeInfo = new BitflyerExchangeInfoApi(publicApi);
        var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
        var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
        var normalized = NormalizedApi.FromRaw(raw, markets);
        return new BitflyerPublicClient(normalized, exchangeInfo);
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
        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? BitflyerHttpPolicyDefaults.Create());
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

        return BitflyerExchangeClient.FromRestClient(restClient);
    }
}
