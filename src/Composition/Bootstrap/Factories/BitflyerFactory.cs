using System;
using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.ExchangeInfo.Api;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Facade;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Facade;

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
        var restClient = CreateRestClient(settings, signer);
        if (signer is null)
        {
            var exchangeInfo = new BitflyerExchangeInfoApi();
            var contractMarkets = new ExchangeInfoMarketResolver(exchangeInfo);
            var markets = new BitflyerNormalizedMarketResolver(contractMarkets);
            var normalized = BitflyerNormalizedApi.FromRestClient(restClient, markets);
            return new BitflyerPublicClient(normalized);
        }

        return BitflyerExchangeClient.FromRestClient(restClient);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BitflyerExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (BitflyerExchangeClient)CreateClient(options);

    private static RestClient CreateRestClient(BitflyerFactoryOptions settings, IRequestSigner? signer)
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

    private static IRequestSigner? CreateSigner(BitflyerFactoryOptions settings)
    {
        var credentials = settings.Credentials
            ?? settings.CredentialProvider?.Get(settings.Exchange, settings.AccountId);

        if (credentials is null)
        {
            return null;
        }

        var clock = settings.Clock ?? new SystemClock();
        return new BitflyerRequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);
    }
}
