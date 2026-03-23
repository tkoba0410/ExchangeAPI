using ExchangeApi.Composition.Bootstrap.Transport;
using ExchangeApi.Stage10.Bitflyer.Composition.Factory;
using ExchangeApi.Stage10.Bitflyer.Composition.Options;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Endpoints.SendChildOrder;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Auth;
using ExchangeApi.Stage10.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTicker;
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
        var publicNative = new BitflyerPublicNativeApi(runtime.PublicNative.GetTicker);
        var privateNative = runtime.PrivateProtocol is null
            ? null
            : new BitflyerPrivateNativeApi(
                runtime.PrivateNative!.GetBalance,
                runtime.PrivateNative.SendChildOrder,
                runtime.PrivateNative.CancelChildOrder);

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
        var publicProtocolModules = new PublicProtocolModules(
            GetTicker: new GetTickerProtocolEndpoint(protocolTransport));
        var publicProtocol = new BitflyerPublicProtocolApi(publicProtocolModules.GetTicker);

        PrivateProtocolModules? privateProtocolModules = null;
        IBitflyerPrivateProtocolApi? privateProtocol = null;
        PrivateNativeModules? privateNativeModules = null;

        if (signer is not null)
        {
            privateProtocolModules = new PrivateProtocolModules(
                GetBalance: new GetBalanceProtocolEndpoint(protocolTransport),
                SendChildOrder: new SendChildOrderProtocolEndpoint(protocolTransport),
                CancelChildOrder: new CancelChildOrderProtocolEndpoint(protocolTransport));
            privateProtocol = new BitflyerPrivateProtocolApi(
                privateProtocolModules.GetBalance,
                privateProtocolModules.SendChildOrder,
                privateProtocolModules.CancelChildOrder);
            privateNativeModules = new PrivateNativeModules(
                GetBalance: new GetBalanceNativeEndpoint(privateProtocolModules.GetBalance),
                SendChildOrder: new SendChildOrderNativeEndpoint(privateProtocolModules.SendChildOrder),
                CancelChildOrder: new CancelChildOrderNativeEndpoint(privateProtocolModules.CancelChildOrder));
        }

        var publicNativeModules = new PublicNativeModules(
            GetTicker: new GetTickerNativeEndpoint(publicProtocolModules.GetTicker));

        return new RuntimeParts(
            restClient,
            publicProtocol,
            privateProtocol,
            publicNativeModules,
            privateNativeModules);
    }

    private sealed record RuntimeParts(
        IDisposable Owner,
        IBitflyerPublicProtocolApi PublicProtocol,
        IBitflyerPrivateProtocolApi? PrivateProtocol,
        PublicNativeModules PublicNative,
        PrivateNativeModules? PrivateNative);

    private sealed record PublicProtocolModules(
        IGetTickerProtocolEndpoint GetTicker);

    private sealed record PrivateProtocolModules(
        IGetBalanceProtocolEndpoint GetBalance,
        ISendChildOrderProtocolEndpoint SendChildOrder,
        ICancelChildOrderProtocolEndpoint CancelChildOrder);

    private sealed record PublicNativeModules(
        IGetTickerNativeEndpoint GetTicker);

    private sealed record PrivateNativeModules(
        IGetBalanceNativeEndpoint GetBalance,
        ISendChildOrderNativeEndpoint SendChildOrder,
        ICancelChildOrderNativeEndpoint CancelChildOrder);
}
