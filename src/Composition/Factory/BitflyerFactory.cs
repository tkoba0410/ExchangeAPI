using System;
using ExchangeApi.Composition.Transport;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Composition.Factory;

/// <summary>
/// bitFlyer 用の標準配線を提供するファクトリ。
/// 既定は Raw クライアントを返し、Adapter は明示的に呼び出した場合のみ生成する。
/// </summary>
public static class BitflyerFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static BitflyerRawApiClient CreateRaw(BitflyerFactoryOptions? options = null)
    {
        var components = CreateRawComponents(options ?? new BitflyerFactoryOptions());
        return new BitflyerRawApiClient(components.PublicApi, components.PrivateApi, components.PrivateTradingApi);
    }

    public static BitflyerExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var components = CreateRawComponents(settings);

        return new BitflyerExchangeClient(
            components.PublicApi,
            components.PrivateApi,
            components.PrivateTradingApi,
            exchangeId: settings.ExchangeId,
            accountId: settings.AccountId);
    }

    private static BitflyerComponents CreateRawComponents(BitflyerFactoryOptions settings)
    {
        var restClient = CreateRestClient(settings);
        return new BitflyerComponents(
            RestClient: restClient,
            PublicApi: new BitflyerPublicApi(restClient),
            PrivateApi: new BitflyerPrivateApi(restClient),
            PrivateTradingApi: new BitflyerPrivateTradingApi(restClient));
    }

    private static RestClient CreateRestClient(BitflyerFactoryOptions settings)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(settings.PolicyOptions);

        return RestClientFactory.Create(
            baseUri,
            transport: settings.Transport,
            signer: settings.RequestSigner ?? CreateSigner(settings),
            policy: policy,
            logger: settings.Logger,
            observer: settings.Observer,
            errorClassifier: settings.ErrorClassifier,
            httpClient: settings.HttpClient);
    }

    private static IRequestSigner? CreateSigner(BitflyerFactoryOptions settings)
    {
        var credentials = settings.Credentials
            ?? settings.CredentialProvider?.Get(settings.ExchangeId, settings.AccountId);

        if (credentials is null)
        {
            return null;
        }

        var clock = settings.Clock ?? new SystemClock();
        return new BitflyerRequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);
    }

    private sealed record BitflyerComponents(
        RestClient RestClient,
        BitflyerPublicApi PublicApi,
        BitflyerPrivateApi PrivateApi,
        BitflyerPrivateTradingApi PrivateTradingApi);
}
