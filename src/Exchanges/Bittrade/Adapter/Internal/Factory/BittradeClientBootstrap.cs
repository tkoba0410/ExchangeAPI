using System;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

internal static class BittradeClientBootstrap
{
    private static readonly Uri DefaultBaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static (BittradeClientComponents Components, IRestClient RestClient) CreatePublicComponents(ClientOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var restClient = CreateRestClient(options, signer: null);
        var components = BittradeClientComponents.FromRestClient(restClient, accountId: null);
        return (components, restClient);
    }

    public static (BittradeClientComponents Components, AccountId AccountId, IRestClient RestClient) CreatePrivateComponents(
        ClientOptions options,
        ClientCredentials credentials,
        string accountId)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        if (credentials is null) throw new ArgumentNullException(nameof(credentials));
        if (string.IsNullOrWhiteSpace(credentials.ApiKey))
        {
            throw new ArgumentException("API key is required.", nameof(credentials));
        }

        if (string.IsNullOrWhiteSpace(credentials.ApiSecret))
        {
            throw new ArgumentException("API secret is required.", nameof(credentials));
        }

        if (string.IsNullOrWhiteSpace(accountId))
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = AccountId.ParseOrThrow(accountId);
        var restClient = CreateRestClient(options, new RequestSigner(credentials.ApiKey, credentials.ApiSecret));
        var components = BittradeClientComponents.FromRestClient(restClient, normalizedAccountId);
        return (components, normalizedAccountId, restClient);
    }

    private static IRestClient CreateRestClient(
        ClientOptions options,
        IRequestSigner? signer)
    {
        var baseUri = options.BaseUri ?? DefaultBaseUri;
        var resolved = TransportConfigResolver.Resolve(baseUri, options.TransportConfig);
        var policyObserver = NoOpPolicyObserver.Instance;
        var policy = options.Policy ?? HttpPolicyFactory.CreateDefault(
            options.PolicyOptions,
            observer: policyObserver);
        return new RestClient(
            baseUri,
            resolved.Transport,
            policy: policy,
            errorClassifier: options.ErrorClassifier ?? ErrorClassifier.Instance,
            requestSigner: signer,
            observer: options.Observer,
            logger: options.Logger,
            disposeTransport: resolved.DisposeTransport);
    }
}
