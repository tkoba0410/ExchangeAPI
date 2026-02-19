using System;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Composition;

/// <summary>
/// Bittrade 用の標準配線ファクトリ。Adapter を既定とし、Spec を参照しない。
/// </summary>
public static class BittradeFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static INormalizedApi CreateClient(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        var credentials = ResolveCredentials(settings);
        var signer = ResolveSigner(settings, credentials);
        if (signer is null)
        {
            throw new InvalidOperationException(
                "Bittrade normalized client requires credentials or RequestSigner. " +
                "Use CreateContractClient(...) for minimal cross-exchange usage.");
        }

        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("Bittrade accountId is required to create a normalized client.");
        }

        var restClient = CreateRestClient(settings, signer);
        var accountId = AccountId.ParseOrThrow(settings.AccountId);
        var components = BittradeClientComponents.FromRestClient(restClient, accountId);
        if (components.Normalized is null)
        {
            throw new InvalidOperationException("Failed to create Bittrade normalized client.");
        }

        return components.Normalized;
    }

    public static IExchangeClient CreateContractClient(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        var adapterOptions = ToAdapterOptions(settings);
        var credentials = ResolveCredentials(settings);

        if (credentials is null)
        {
            return new PublicClient(adapterOptions);
        }

        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("Bittrade accountId is required to create a private client.");
        }

        var adapterCredentials = new ClientCredentials(credentials.ApiKey, credentials.ApiSecret);
        return new ExchangeClient(adapterOptions, adapterCredentials, settings.AccountId);
    }

    [Obsolete("Use CreateContractClient(...) instead. This method will be removed in a future major release.")]
    internal static ExchangeClient CreateAdapter(BittradeFactoryOptions? options = null) =>
        (ExchangeClient)CreateContractClient(options);

    private static ClientOptions ToAdapterOptions(BittradeFactoryOptions settings)
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

    private static RestClient CreateRestClient(BittradeFactoryOptions settings, IRequestSigner? signer)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(
            settings.PolicyOptions);

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

    private static ApiCredentials? ResolveCredentials(BittradeFactoryOptions settings)
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

    private static IRequestSigner? ResolveSigner(BittradeFactoryOptions settings, ApiCredentials? credentials)
    {
        if (settings.RequestSigner is not null)
        {
            return settings.RequestSigner;
        }

        if (credentials is null)
        {
            return null;
        }

        return new RequestSigner(credentials.ApiKey, credentials.ApiSecret);
    }
}
