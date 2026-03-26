using ExchangeApi.Exchanges.Binance.Composition.Factory;
using ExchangeApi.Exchanges.Binance.Composition.Internal.Runtime;
using ExchangeApi.Exchanges.Binance.Composition.Options;
using ExchangeApi.Exchanges.Binance.Native.Public.Api;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;

namespace ExchangeApi.Exchanges.Binance.Composition.Bootstrap;

internal static class BinanceBootstrap
{
    internal static BinanceProtocolBundle CreateProtocolBundle(BinanceClientOptions? options)
    {
        var normalizedOptions = ValidateOptions(options ?? new BinanceClientOptions());
        var runtime = CreateInternalTransportRuntime(normalizedOptions);
        var transport = runtime.Transport;

        var getKlines = new GetKlinesProtocolEndpoint(transport);
        var publicApi = new BinancePublicProtocolApi(getKlines);

        return new BinanceProtocolBundle
        {
            Public = publicApi,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };
    }

    internal static BinanceProtocolBundle CreateProtocolBundle(HttpClient httpClient, BinanceClientOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var normalizedOptions = ValidateOptions(options ?? new BinanceClientOptions());
        var runtime = CreateExternalTransportRuntime(httpClient, normalizedOptions);
        var transport = runtime.Transport;

        var getKlines = new GetKlinesProtocolEndpoint(transport);
        var publicApi = new BinancePublicProtocolApi(getKlines);

        return new BinanceProtocolBundle
        {
            Public = publicApi,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };
    }

    internal static BinanceNativeBundle CreateNativeBundle(BinanceClientOptions? options)
    {
        var normalizedOptions = ValidateOptions(options ?? new BinanceClientOptions());
        var runtime = CreateInternalTransportRuntime(normalizedOptions);
        var transport = runtime.Transport;

        var getKlinesProtocol = new GetKlinesProtocolEndpoint(transport);
        var protocolApi = new BinancePublicProtocolApi(getKlinesProtocol);
        var getKlinesNative = new GetKlinesNativeEndpoint(getKlinesProtocol);
        var publicApi = new BinancePublicNativeApi(getKlinesNative);
        var protocolBundle = new BinanceProtocolBundle
        {
            Public = protocolApi,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };

        return new BinanceNativeBundle
        {
            Public = publicApi,
            Protocol = protocolBundle,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };
    }

    internal static BinanceNativeBundle CreateNativeBundle(HttpClient httpClient, BinanceClientOptions? options)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        var normalizedOptions = ValidateOptions(options ?? new BinanceClientOptions());
        var runtime = CreateExternalTransportRuntime(httpClient, normalizedOptions);
        var transport = runtime.Transport;

        var getKlinesProtocol = new GetKlinesProtocolEndpoint(transport);
        var protocolApi = new BinancePublicProtocolApi(getKlinesProtocol);
        var getKlinesNative = new GetKlinesNativeEndpoint(getKlinesProtocol);
        var publicApi = new BinancePublicNativeApi(getKlinesNative);
        var protocolBundle = new BinanceProtocolBundle
        {
            Public = protocolApi,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };

        return new BinanceNativeBundle
        {
            Public = publicApi,
            Protocol = protocolBundle,
            LifetimeLease = runtime.Lifetime.AcquireLease(),
        };
    }

    private static BinanceClientOptions ValidateOptions(BinanceClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (!options.BaseUri.IsAbsoluteUri)
        {
            throw new ArgumentException("BaseUri must be absolute.", nameof(options));
        }

        if (options.RequestTimeout is not null && options.RequestTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(BinanceClientOptions.RequestTimeout), "RequestTimeout must be greater than zero.");
        }

        return options;
    }

    private static TransportRuntime CreateInternalTransportRuntime(BinanceClientOptions options)
    {
        var httpClient = new HttpClient
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };

        return CreateTransportRuntime(httpClient, options, SharedBundleLifetime.CreateOwned(httpClient));
    }

    private static TransportRuntime CreateExternalTransportRuntime(HttpClient httpClient, BinanceClientOptions options)
    {
        return CreateTransportRuntime(httpClient, options, SharedBundleLifetime.CreateExternal());
    }

    private static TransportRuntime CreateTransportRuntime(
        HttpClient httpClient,
        BinanceClientOptions options,
        SharedBundleLifetime lifetime)
    {
        IProtocolDebugLogger debugLogger = options.EnableProtocolDebugLogging
            ? new FileProtocolDebugLogger(options.ProtocolDebugLogDirectory)
            : new NoOpProtocolDebugLogger();

        return new TransportRuntime
        {
            Transport = new BinanceProtocolTransport(httpClient, options.BaseUri, debugLogger, options.RequestTimeout),
            Lifetime = lifetime,
        };
    }

    private sealed class TransportRuntime
    {
        public required IProtocolTransport Transport { get; init; }
        public required SharedBundleLifetime Lifetime { get; init; }
    }
}
