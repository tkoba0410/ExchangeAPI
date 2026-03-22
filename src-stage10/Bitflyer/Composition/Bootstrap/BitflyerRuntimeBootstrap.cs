using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Normalized.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Stage10.Bitflyer.Wire.Internal.Auth;
using ExchangeApi.Stage10.Bitflyer.Wire.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Wire.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Bootstrap;

internal static class BitflyerRuntimeBootstrap
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static BitflyerWireClientBundle CreateWireBundle(BitflyerStage10ClientOptions options)
    {
        var runtime = BuildRuntime(options);

        return new BitflyerWireClientBundle(
            runtime.Owner,
            runtime.PublicWire,
            runtime.PrivateWire);
    }

    public static BitflyerNormalizedClientBundle CreateNormalizedBundle(BitflyerStage10ClientOptions options)
    {
        var runtime = BuildRuntime(options);
        var publicNormalized = new BitflyerPublicNormalizedApi(runtime.PublicWire);
        var privateNormalized = runtime.PrivateWire is null
            ? null
            : new BitflyerPrivateNormalizedApi(runtime.PrivateWire);

        return new BitflyerNormalizedClientBundle(
            runtime.Owner,
            new BitflyerWireClientView(runtime.PublicWire, runtime.PrivateWire),
            publicNormalized,
            privateNormalized);
    }

    private static RuntimeParts BuildRuntime(BitflyerStage10ClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var baseUri = options.BaseUri ?? DefaultBaseUri;
        var transportConfig = options.TransportConfig ?? new Transport.Http.TransportConfig.ManagedHttp();
        IRequestSigner? signer = null;

        if (options.Credentials is not null)
        {
            signer = new BitflyerRequestSigner(
                options.Credentials.ApiKey,
                options.Credentials.ApiSecret,
                new SystemClock());
        }

        var restClient = RestClientFactory.Create(
            baseUri,
            transportConfig,
            signer: signer,
            policy: NoOpHttpPolicy.Instance,
            logger: NoOpRestClientLogger.Instance,
            observer: NoOpRestCallObserver.Instance);

        var wireTransport = new BitflyerWireTransport(restClient, options.UseTickerAliasPath);
        var publicWire = new BitflyerPublicWireApi(wireTransport);
        var privateWire = signer is null ? null : new BitflyerPrivateWireApi(wireTransport);

        return new RuntimeParts(restClient, publicWire, privateWire);
    }

    private sealed record RuntimeParts(
        IDisposable Owner,
        IBitflyerPublicWireApi PublicWire,
        IBitflyerPrivateWireApi? PrivateWire);
}
