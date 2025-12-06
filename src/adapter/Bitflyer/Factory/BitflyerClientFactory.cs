using System;
using System.Net.Http;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Transport.Logging;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Transport.Transport;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
    /// Factory for constructing bitFlyer client instances.
    /// HttpClient -> HttpTransport -> RestClient(署名/ポリシー/ログ) -> BitflyerPublicApi/BitflyerPrivateApi -> BitflyerExchangeClient.
    /// </summary>
    public static class BitflyerClientFactory
    {
        private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller.
    /// </summary>
    public static BitflyerExchangeClient Create(
        string apiKey,
        string apiSecret,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        HttpClient? httpClient = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(apiSecret));
        }

        httpClient ??= new HttpClient { BaseAddress = BitflyerApiBaseUri };

        IHttpTransport baseTransport = new HttpTransport(httpClient, disposeHttpClient: false);

        IExchangeClock clock = new SystemClock();

        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);

        IRestClient restClient = new RestClient(
            BitflyerApiBaseUri,
            baseTransport,
            requestSigner: signer,
            policy: policy,
            logger: logger);

        var publicApi = new BitflyerPublicApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);
        var rawApi = new BitflyerRawApiClient(publicApi, privateApi, privateApi);

        return new BitflyerExchangeClient(publicApi, privateApi, privateApi);
    }

    /// <summary>
    /// Create bitFlyer client using a credential provider to retrieve API key/secret.
    /// </summary>
    public static BitflyerExchangeClient Create(
        IApiCredentialProvider provider,
        string exchangeId,
        string accountId,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        HttpClient? httpClient = null)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var credentials = provider.Get(exchangeId, accountId);
        return Create(credentials.ApiKey, credentials.ApiSecret, policy, logger, httpClient);
    }
}
