using System;
using System.Net.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Transport.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

/// <summary>
/// Factory for constructing bitFlyer client instances.
/// HttpClient -> HttpTransport -> RestClient(署名/ポリシー/ログ) -> Raw/Private API -> ExchangeClient.
/// </summary>
public static class ClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// Public API のみを利用する軽量クライアントを作成する。
    /// 署名を行わず、マーケット取得に限定する。
    /// </summary>
    public static PublicClient CreatePublic(
        ClientOptions options,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var baseUri = options.BaseUri ?? BitflyerApiBaseUri;
        var http = httpClient ?? options.HttpClient ?? new HttpClient { BaseAddress = baseUri };
        if (options.Timeout is { } timeout)
        {
            http.Timeout = timeout;
        }
        IHttpTransport baseTransport = transportOverride ?? new HttpTransport(http, disposeHttpClient: false);

        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? HttpPolicyDefaults.Create());
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? ErrorClassifier.Instance;

        IRestClient restClient = new RestClient(
            baseUri,
            baseTransport,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier);

        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new PublicClient(components.Normalized);
    }

    /// <summary>
    /// Public API のみを利用する軽量クライアントを作成する。
    /// </summary>
    [Obsolete("Pass ClientOptions explicitly. This overload will be removed in a future major release.")]
    public static PublicClient CreatePublic(
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null) =>
        CreatePublic(new ClientOptions(), httpClient, transportOverride);

    /// <summary>
    /// Create bitFlyer client with explicit credentials supplied by the caller.
    /// </summary>
    public static ExchangeClient Create(
        ClientCredentials credentials,
        ClientOptions options,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null)
    {
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));
        if (string.IsNullOrWhiteSpace(credentials.ApiKey))
        {
            throw new ArgumentException("API key is required.", nameof(credentials));
        }

        if (string.IsNullOrWhiteSpace(credentials.ApiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(credentials));
        }

        if (options is null) throw new ArgumentNullException(nameof(options));

        var baseUri = options.BaseUri ?? BitflyerApiBaseUri;
        var http = httpClient ?? options.HttpClient ?? new HttpClient { BaseAddress = baseUri };
        if (options.Timeout is { } timeout)
        {
            http.Timeout = timeout;
        }

        IHttpTransport baseTransport = transportOverride ?? new HttpTransport(http, disposeHttpClient: false);

        IExchangeClock clock = new SystemClock();
        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? HttpPolicyDefaults.Create());
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? ErrorClassifier.Instance;

        IRequestSigner signer = new RequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);

        IRestClient restClient = new RestClient(
            baseUri,
            baseTransport,
            requestSigner: signer,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier);

        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new ExchangeClient(components.Normalized, components.Markets);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller.
    /// </summary>
    [Obsolete("Use Create(ClientCredentials, ClientOptions, ...) instead. This overload will be removed in a future major release.")]
    public static ExchangeClient Create(
        string apiKey,
        string apiSecret,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null,
        HttpClient? httpClient = null)
    {
        var options = new ClientOptions
        {
            Policy = policy,
            Logger = logger,
            Observer = observer,
            ErrorClassifier = errorClassifier,
            HttpClient = httpClient,
        };

        return Create(new ClientCredentials(apiKey, apiSecret), options, httpClient: httpClient);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller using options.
    /// </summary>
    [Obsolete("Use Create(ClientCredentials, ClientOptions, ...) instead. This overload will be removed in a future major release.")]
    public static ExchangeClient Create(
        string apiKey,
        string apiSecret,
        ClientOptions? options,
        HttpClient? httpClient = null,
        IHttpTransport? transportOverride = null) =>
        Create(new ClientCredentials(apiKey, apiSecret), options ?? new ClientOptions(), httpClient, transportOverride);
}
