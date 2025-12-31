using System;
using ExchangeApi.Composition.Transport;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Time;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using Raw = ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Contracts.Interfaces;

namespace ExchangeApi.Composition.Factory;

/// <summary>
/// bitFlyer 用の標準配線を提供するファクトリ。
/// Raw を既定とし、Adapter は明示的に opt-in する。
/// </summary>
public static class BitflyerFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static Raw.BitflyerRawApi CreateRaw(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var restClient = CreateRestClient(settings);
        var wire = new Raw.Internal.Wire.BitflyerWireApi(restClient);
        return new Raw.BitflyerRawApi(wire);
    }

    /// <summary>
    /// bitFlyer の Normalized 入口を作成する。
    /// Raw -> Normalize までを提供し、Contracts/Adapter は返さない。
    /// </summary>
    public static BitflyerNormalizedApi CreateExchange(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var restClient = CreateRestClient(settings);
        return BitflyerNormalizedApi.FromRestClient(restClient);
    }

    public static IExchangeClient CreateClient(BitflyerFactoryOptions? options = null)
    {
        var settings = options ?? new BitflyerFactoryOptions();
        var components = CreateComponents(settings);

        return new BitflyerExchangeClient(
            components.MarketData,
            components.PrivateApi,
            components.TradingApi,
            exchangeCode: settings.Exchange,
            accountId: settings.AccountId);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BitflyerExchangeClient CreateAdapter(BitflyerFactoryOptions? options = null) =>
        (BitflyerExchangeClient)CreateClient(options);

    private static BitflyerComponents CreateComponents(BitflyerFactoryOptions settings)
    {
        var restClient = CreateRestClient(settings);
        var wire = new Raw.Internal.Wire.BitflyerWireApi(restClient);
        var raw = new Raw.BitflyerRawApi(wire);
        var privateApi = new BitflyerPrivateApi(wire.Account);
        var legacyTradingApi = new BitflyerPrivateTradingApi(wire.Trading);
        return new BitflyerComponents(
            RestClient: restClient,
            Raw: raw,
            MarketData: raw.MarketData,
            PrivateApi: privateApi,
            TradingApi: legacyTradingApi);
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
        Raw.IBitflyerRawMarketDataApi MarketData,
        BitflyerPrivateApi PrivateApi,
        IBitflyerRawPrivateTradingApi TradingApi);
}
