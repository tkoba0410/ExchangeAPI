using System;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal.Auth;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Error;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Bootstrap;

/// <summary>
/// Factory for constructing bitFlyer client instances.
/// TransportConfig -> IHttpTransport -> RestClient(署名/ポリシー/ログ) -> Raw/Private API -> ExchangeClient.
/// </summary>
public static class ClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// Public API のみを利用する軽量クライアントを作成する。
    /// 署名を行わず、マーケット取得に限定する。
    /// </summary>
    public static PublicClient CreatePublic(ClientOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var baseUri = options.BaseUri ?? BitflyerApiBaseUri;
        var resolved = TransportConfigResolver.Resolve(baseUri, options.TransportConfig);

        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? HttpPolicyDefaults.Create());
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? ErrorClassifier.Instance;

        IRestClient restClient = new RestClient(
            baseUri,
            resolved.Transport,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier,
            disposeTransport: resolved.DisposeTransport);

        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new PublicClient(components.Normalized, restClient);
    }

    /// <summary>
    /// Public API のみを利用する軽量クライアントを作成する。
    /// </summary>
    [Obsolete("Pass ClientOptions explicitly. This overload will be removed in a future major release.")]
    public static PublicClient CreatePublic() =>
        CreatePublic(new ClientOptions());

    /// <summary>
    /// Create bitFlyer client with explicit credentials supplied by the caller.
    /// </summary>
    public static ExchangeClient Create(
        ClientCredentials credentials,
        ClientOptions options)
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
        var resolved = TransportConfigResolver.Resolve(baseUri, options.TransportConfig);

        IExchangeClock clock = new SystemClock();
        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions ?? HttpPolicyDefaults.Create());
        var logger = options.Logger;
        var observer = options.Observer;
        var errorClassifier = options.ErrorClassifier ?? ErrorClassifier.Instance;

        IRequestSigner signer = new RequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);

        IRestClient restClient = new RestClient(
            baseUri,
            resolved.Transport,
            requestSigner: signer,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier,
            disposeTransport: resolved.DisposeTransport);

        var components = BitflyerClientComponents.FromRestClient(restClient);
        return new ExchangeClient(components.Normalized, components.Markets, restClient);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller.
    /// </summary>
    [Obsolete("Use Create(ClientCredentials, ClientOptions) instead. This overload will be removed in a future major release.")]
    public static ExchangeClient Create(
        string apiKey,
        string apiSecret,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null)
    {
        var options = new ClientOptions
        {
            Policy = policy,
            Logger = logger,
            Observer = observer,
            ErrorClassifier = errorClassifier,
        };

        return Create(new ClientCredentials(apiKey, apiSecret), options);
    }

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller using options.
    /// </summary>
    [Obsolete("Use Create(ClientCredentials, ClientOptions) instead. This overload will be removed in a future major release.")]
    public static ExchangeClient Create(
        string apiKey,
        string apiSecret,
        ClientOptions? options) =>
        Create(new ClientCredentials(apiKey, apiSecret), options ?? new ClientOptions());
}
