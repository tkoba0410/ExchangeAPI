using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Auth;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Stage10.Bitflyer.Composition.Bootstrap;

internal static class BitflyerRuntimeBootstrap
{
    private static readonly Uri DefaultBaseUri = new("https://api.bitflyer.com");

    public static BitflyerProtocolClientBundle CreateProtocolBundle(BitflyerStage10ClientOptions options)
    {
        var runtime = BuildRuntime(options);

        return new BitflyerProtocolClientBundle(
            runtime.Owner,
            runtime.PublicProtocol,
            runtime.PrivateProtocol);
    }

    public static BitflyerNativeClientBundle CreateNativeBundle(BitflyerStage10ClientOptions options)
    {
        var runtime = BuildRuntime(options);
        var publicNative = new BitflyerPublicNativeApi(runtime.PublicProtocol);
        var privateNative = runtime.PrivateProtocol is null
            ? null
            : new BitflyerPrivateNativeApi(runtime.PrivateProtocol);

        return new BitflyerNativeClientBundle(
            runtime.Owner,
            new BitflyerProtocolClientView(runtime.PublicProtocol, runtime.PrivateProtocol),
            publicNative,
            privateNative);
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

        var protocolTransport = new BitflyerProtocolTransport(restClient, options.UseTickerAliasPath);
        var publicProtocol = new BitflyerPublicProtocolApi(protocolTransport);
        var privateProtocol = signer is null ? null : new BitflyerPrivateProtocolApi(protocolTransport);

        return new RuntimeParts(restClient, publicProtocol, privateProtocol);
    }

    private sealed record RuntimeParts(
        IDisposable Owner,
        IBitflyerPublicProtocolApi PublicProtocol,
        IBitflyerPrivateProtocolApi? PrivateProtocol);
}
