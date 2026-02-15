using System;
using System.Net.Http;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Composition;

/// <summary>
/// bitFlyer 用の標準配線を提供するファクトリ。
/// Adapter を既定とし、Spec を参照しない。
/// </summary>
public static class BitflyerFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static IExchangeClient CreateClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var clientOptions = ToClientOptions(settings);
        // 署名（認証）が無い場合は PublicClient を返し、未対応 capability は null とする。
        var credentials = ResolveCredentials(settings);
        if (credentials is null && settings.RequestSigner is null)
        {
            return new PublicClient(clientOptions, settings.HttpClient, settings.Transport);
        }

        if (settings.RequestSigner is not null)
        {
            var restClient = CreateRestClient(settings, settings.RequestSigner);
            return ExchangeClient.FromRestClient(restClient);
        }

        var clientCredentials = new ClientCredentials(credentials!.ApiKey, credentials.ApiSecret);
        return new ExchangeClient(clientOptions, clientCredentials, settings.HttpClient, settings.Transport);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static ExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (ExchangeClient)CreateClient(options);

    private static ClientOptions ToClientOptions(BitflyerFactoryOptions settings)
    {
        if (settings is null) throw new ArgumentNullException(nameof(settings));
        return new ClientOptions
        {
            BaseUri = settings.BaseUri ?? DefaultBaseUri,
            HttpClient = settings.HttpClient,
            Policy = settings.Policy,
            PolicyOptions = settings.PolicyOptions,
            Logger = settings.Logger,
            Observer = settings.Observer,
            ErrorClassifier = settings.ErrorClassifier,
        };
    }

    private static RestClient CreateRestClient(BitflyerFactoryOptions settings, IRequestSigner? signer)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(
            settings.PolicyOptions ?? HttpPolicyDefaults.Create());

        return RestClientFactory.Create(
            baseUri,
            transport: settings.Transport,
            signer: signer,
            policy: policy,
            logger: settings.Logger,
            observer: settings.Observer,
            errorClassifier: settings.ErrorClassifier,
            httpClient: settings.HttpClient);
    }

    private static ApiCredentials? ResolveCredentials(BitflyerFactoryOptions settings)
    {
        var credentials = settings.Credentials;
        if (credentials is null && settings.CredentialProvider is not null)
        {
            var accountId = AccountId.ParseOrThrow(settings.AccountId);
            credentials = settings.CredentialProvider.Get(accountId);
        }

        if (credentials is null)
        {
            return null;
        }
        return credentials;
    }
}
