using System;
using System.Net.Http;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal.Factory;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Private.Api;

namespace ExchangeApi.Composition.Bootstrap.Factories;

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
        // 署名（認証）が無い場合は PublicClient を返し、未対応 capability は null とする。
        var signer = settings.RequestSigner ?? CreateSigner(settings);
        if (signer is null)
        {
            var httpClient = settings.HttpClient;
            if (httpClient is null && settings.BaseUri is not null)
            {
                httpClient = new HttpClient { BaseAddress = settings.BaseUri };
            }

            var clientOptions = new BitflyerClientOptions
            {
                HttpClient = httpClient,
                Policy = settings.Policy,
                PolicyOptions = settings.PolicyOptions,
                Logger = settings.Logger,
                Observer = settings.Observer,
                ErrorClassifier = settings.ErrorClassifier,
            };

            return BitflyerClientFactory.CreatePublic(
                clientOptions,
                httpClient: httpClient,
                transportOverride: settings.Transport);
        }

        var restClient = CreateRestClient(settings, signer);
        return BitflyerExchangeClient.FromRestClient(restClient);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BitflyerExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (BitflyerExchangeClient)CreateClient(options);

    private static RestClient CreateRestClient(BitflyerFactoryOptions settings, IRequestSigner? signer)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(
            settings.PolicyOptions ?? BitflyerHttpPolicyDefaults.Create());

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

    private static IRequestSigner? CreateSigner(BitflyerFactoryOptions settings)
    {
        var credentials = settings.Credentials
            ?? settings.CredentialProvider?.Get(settings.AccountId);

        if (credentials is null)
        {
            return null;
        }

        var clock = settings.Clock ?? new SystemClock();
        return new BitflyerRequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);
    }
}
