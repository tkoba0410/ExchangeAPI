using System;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
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
        var signer = settings.RequestSigner ?? CreateSigner(settings, requireCredentials: false);
        var restClient = CreateRestClient(settings, signer);

        // 認証が無い場合は PublicClient を返す（Private capability は提供しない）。
        if (signer is null)
        {
            return new PublicClient(restClient);
        }

        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("Bittrade accountId is required to create a private client.");
        }

        var accountId = AccountId.ParseOrThrow(settings.AccountId);
        var bundle = ApiBundle.FromRestClient(restClient, accountId);
        return new ExchangeClient(bundle);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static ExchangeClient CreateAdapter(BittradeFactoryOptions? options = null) =>
        (ExchangeClient)CreateClient(options);

    private static RestClient CreateRestClient(BittradeFactoryOptions settings, IRequestSigner? signer)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(settings.PolicyOptions);

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

    private static IRequestSigner? CreateSigner(BittradeFactoryOptions settings, bool requireCredentials)
    {
        var credentials = settings.Credentials;
        if (credentials is null && settings.CredentialProvider is not null)
        {
            var accountId = AccountId.ParseOrThrow(settings.AccountId);
            credentials = settings.CredentialProvider.Get(accountId);
        }

        if (credentials is null)
        {
            if (requireCredentials)
            {
                throw new InvalidOperationException("Bittrade credentials are required to create the client.");
            }

            return null;
        }

        return new RequestSigner(credentials.ApiKey, credentials.ApiSecret);
    }
}
