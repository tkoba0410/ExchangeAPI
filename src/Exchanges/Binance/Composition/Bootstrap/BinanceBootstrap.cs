using ExchangeApi.Exchanges.Binance.Composition.Factory;
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
        var normalizedOptions = options ?? new BinanceClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getKlines = new GetKlinesProtocolEndpoint(transport);
        var publicApi = new BinancePublicProtocolApi(getKlines);

        return new BinanceProtocolBundle
        {
            Public = publicApi,
        };
    }

    internal static BinanceNativeBundle CreateNativeBundle(BinanceClientOptions? options)
    {
        var normalizedOptions = options ?? new BinanceClientOptions();
        var transport = CreateTransport(normalizedOptions);

        var getKlinesProtocol = new GetKlinesProtocolEndpoint(transport);
        var protocolApi = new BinancePublicProtocolApi(getKlinesProtocol);
        var getKlinesNative = new GetKlinesNativeEndpoint(getKlinesProtocol);
        var publicApi = new BinancePublicNativeApi(getKlinesNative);

        return new BinanceNativeBundle
        {
            Public = publicApi,
            Protocol = new BinanceProtocolBundle
            {
                Public = protocolApi,
            },
        };
    }

    private static IProtocolTransport CreateTransport(BinanceClientOptions options)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = options.BaseUri,
        };

        IProtocolDebugLogger debugLogger = options.EnableProtocolDebugLogging
            ? new FileProtocolDebugLogger(options.ProtocolDebugLogDirectory)
            : new NoOpProtocolDebugLogger();

        return new BinanceProtocolTransport(httpClient, debugLogger);
    }
}
