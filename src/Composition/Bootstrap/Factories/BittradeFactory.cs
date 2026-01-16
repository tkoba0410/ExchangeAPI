using System;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Internal;
using ExchangeApi.Contracts.Facade.Interfaces;

namespace ExchangeApi.Composition.Bootstrap.Factories;

/// <summary>
/// Bittrade 用の標準配線ファクトリ。Adapter を既定とし、Spec を参照しない。
/// </summary>
public static class BittradeFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IExchangeClient CreateClient(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("BittradeFactoryOptions.AccountId must be specified to create a client.");
        }

        var restClient = CreateRestClient(settings, requireCredentials: true);
        var bundle = BittradeApiBundle.FromRestClient(restClient, settings.AccountId);
        return new BittradeExchangeClient(bundle);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BittradeExchangeClient CreateAdapter(BittradeFactoryOptions? options = null) =>
        (BittradeExchangeClient)CreateClient(options);

    private static RestClient CreateRestClient(BittradeFactoryOptions settings, bool requireCredentials)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(settings.PolicyOptions);

        return RestClientFactory.Create(
            baseUri,
            transport: settings.Transport,
            signer: settings.RequestSigner ?? CreateSigner(settings, requireCredentials),
            policy: policy,
            logger: settings.Logger,
            observer: settings.Observer,
            errorClassifier: settings.ErrorClassifier,
            httpClient: settings.HttpClient);
    }

    private static IRequestSigner? CreateSigner(BittradeFactoryOptions settings, bool requireCredentials)
    {
        var credentials = settings.Credentials
            ?? settings.CredentialProvider?.Get(settings.Exchange, settings.AccountId);

        if (credentials is null)
        {
            if (requireCredentials)
            {
                throw new InvalidOperationException("Bittrade credentials are required to create the client.");
            }

            return null;
        }

        return new BittradeRequestSigner(credentials.ApiKey, credentials.ApiSecret);
    }
}
