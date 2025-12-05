using System;
using System.Net.Http;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Transport.Transport;

namespace ExchangeApi.Adapter.Bitflyer;

/// <summary>
/// Factory for constructing bitFlyer client instances.
/// HttpClient -> HttpTransport -> BitflyerSigningTransport -> RestClient -> BitflyerPublicApi/BitflyerPrivateApi -> BitflyerExchangeClient.
/// </summary>
public static class BitflyerClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    /// <summary>
    /// Create bitFlyer client with explicit API key/secret supplied by the caller.
    /// </summary>
    public static BitflyerExchangeClient Create(string apiKey, string apiSecret)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("API key is required.", nameof(apiKey));
        }

        if (string.IsNullOrWhiteSpace(apiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(apiSecret));
        }

        var httpClient = new HttpClient
        {
            BaseAddress = BitflyerApiBaseUri,
        };

        IHttpTransport baseTransport = new HttpTransport(httpClient, disposeHttpClient: true);

        IExchangeClock clock = new SystemClock();

        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);

        IHttpTransport signingTransport = new BitflyerSigningTransport(baseTransport, signer);

        IRestClient restClient = new RestClient(BitflyerApiBaseUri, signingTransport);

        var publicApi = new BitflyerPublicApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);

        return new BitflyerExchangeClient(publicApi, privateApi, privateApi);
    }

    /// <summary>
    /// Create bitFlyer client using a credential provider to retrieve API key/secret.
    /// </summary>
    public static BitflyerExchangeClient Create(IApiCredentialProvider provider, string exchangeId, string accountId)
    {
        if (provider is null)
        {
            throw new ArgumentNullException(nameof(provider));
        }

        var credentials = provider.Get(exchangeId, accountId);
        return Create(credentials.ApiKey, credentials.ApiSecret);
    }
}
