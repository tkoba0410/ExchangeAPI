using System;
using System.Net.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Http;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

/// <summary>
/// テスト専用のファクトリ。公開 API を汚さずにモックやカスタム Transport を注入できる。
/// </summary>
internal static class TestClientFactory
{
    private static readonly Uri BitflyerApiBaseUri = new("https://api.bitflyer.com");

    public static ExchangeClient Create(ApiBundle bundle)
    {
        return new ExchangeClient(bundle);
    }

    public static ExchangeClient Create(IRestClient restClient)
    {
        return new ExchangeClient(ApiBundle.FromRestClient(restClient));
    }

    public static ExchangeClient CreateWithTransport(
        IHttpTransport transport,
        IRequestSigner? signer = null,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null)
    {
        if (transport is null) throw new ArgumentNullException(nameof(transport));

        var restClient = new RestClient(
            BitflyerApiBaseUri,
            transport,
            requestSigner: signer,
            policy: policy ?? HttpPolicyFactory.CreateDefault(),
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier ?? ErrorClassifier.Instance);

        return new ExchangeClient(ApiBundle.FromRestClient(restClient));
    }
}
