using System;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
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

    public static INormalizedApi CreateClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var credentials = ResolveCredentials(settings);
        var signer = ResolveSigner(settings, credentials);
        if (signer is null)
        {
            throw new InvalidOperationException(
                "Bitflyer normalized client requires credentials or RequestSigner. " +
                "Use CreateContractPublicClient(...) for minimal cross-exchange usage.");
        }

        var restClient = CreateRestClient(settings, signer);
        var components = BitflyerClientComponents.FromRestClient(restClient);
        return components.Normalized;
    }

    public static IContractPublicClient CreateContractPublicClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var clientOptions = ToClientOptions(settings);
        return new PublicClient(clientOptions, settings.HttpClient, settings.Transport);
    }

    public static IContractPrivateClient CreateContractPrivateClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var credentials = ResolveCredentials(settings);
        var signer = ResolveSigner(settings, credentials);
        if (signer is null)
        {
            throw new InvalidOperationException(
                "Bitflyer contract private client requires credentials or RequestSigner.");
        }

        var restClient = CreateRestClient(settings, signer);
        return ExchangeClient.FromRestClient(restClient);
    }

    [Obsolete("Use CreateContractPrivateClient(...) instead. This method will be removed in a future major release.")]
    internal static ExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (ExchangeClient)CreateContractPrivateClient(options);

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

    private static IRequestSigner? ResolveSigner(BitflyerFactoryOptions settings, ApiCredentials? credentials)
    {
        if (settings.RequestSigner is not null)
        {
            return settings.RequestSigner;
        }

        if (credentials is null)
        {
            return null;
        }

        var clock = settings.Clock ?? new SystemClock();
        return new RequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);
    }
}
