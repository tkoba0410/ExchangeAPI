using System;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Composition;

/// <summary>
/// Bittrade 用の標準配線ファクトリ。Adapter を既定とし、Spec を参照しない。
/// </summary>
public static class BittradeFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IExchangeClient CreateClient(BittradeFactoryOptions? options = null)
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

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static ExchangeClient CreateAdapter(BittradeFactoryOptions? options = null) =>
        (ExchangeClient)CreateClient(options);

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
}
