using System;
using ExchangeApi.Composition.Transport;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using ExchangeApi.Contracts.Interfaces;

namespace ExchangeApi.Composition.Factory;

/// <summary>
/// bitFlyer 用の標準配線を提供するファクトリ。
/// Raw を既定とし、Adapter / Wire は明示的に opt-in する。
/// </summary>
public static class BitflyerFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static Raw.BitflyerRawApi CreateRaw(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var restClient = CreateRestClient(settings);
        return new Raw.BitflyerRawApi(restClient);
    }

    public static IExchangeClient CreateClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var components = CreateWireComponents(settings);

        return new BitflyerExchangeClient(
            components.PublicApi,
            components.PrivateApi,
            components.TradingApi,
            components.PublicApi,
            exchangeCode: settings.Exchange,
            accountId: settings.AccountId);
    }

    public static BitflyerWireApi CreateExchange(BitflyerFactoryOptions? options = null)
    {
        var components = CreateWireComponents(options ?? new BitflyerFactoryOptions());
        return new BitflyerWireApi(components.Raw, components.RestClient);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BitflyerExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (BitflyerExchangeClient)CreateClient(options);

    private static BitflyerComponents CreateWireComponents(BitflyerFactoryOptions settings)
    {
        var restClient = CreateRestClient(settings);
        var raw = new Raw.BitflyerRawApi(restClient);
        var publicApi = new BitflyerPublicApi(raw.MarketData);
        var privateApi = new BitflyerPrivateApi(restClient);
        var legacyTradingApi = new BitflyerPrivateTradingApi(restClient);
        var wireTradingApi = new ExchangeApi.Exchanges.Bitflyer.Wire.Private.BitflyerWireTradingApi(
            raw.Trading,
            legacyTradingApi);
        return new BitflyerComponents(
            RestClient: restClient,
            Raw: raw,
            PublicApi: publicApi,
            PrivateApi: privateApi,
            TradingApi: wireTradingApi);
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
            ?? settings.CredentialProvider?.Get(settings.Exchange, settings.AccountId);

        if (credentials is null)
        {
            return null;
        }

        var clock = settings.Clock ?? new SystemClock();
        return new Raw.BitflyerRequestSigner(credentials.ApiKey, credentials.ApiSecret, clock);
    }

    private sealed record BitflyerComponents(
        RestClient RestClient,
        Raw.BitflyerRawApi Raw,
        BitflyerPublicApi PublicApi,
        BitflyerPrivateApi PrivateApi,
        IBitflyerWireTradingApi TradingApi);
}
