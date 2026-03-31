using ExchangeApi.Exchanges.Binance.Composition.Bootstrap;
using ExchangeApi.Exchanges.Binance.Composition.Options;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

/// <summary>
/// Creates Binance protocol and native client bundles from the Composition layer.
/// </summary>
public static class BinanceClientFactory
{
    /// <summary>
    /// Creates a protocol bundle with an internally managed <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A protocol bundle for raw/debug-oriented Binance calls.</returns>
    public static BinanceProtocolBundle CreateProtocolClient(BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateProtocolBundle(options);
    }

    /// <summary>
    /// Creates a protocol bundle using the provided <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The caller-owned HTTP client.</param>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A protocol bundle for raw/debug-oriented Binance calls.</returns>
    public static BinanceProtocolBundle CreateProtocolClient(HttpClient httpClient, BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateProtocolBundle(httpClient, options);
    }

    /// <summary>
    /// Creates a native bundle with an internally managed <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A native bundle for exchange-native Binance calls.</returns>
    public static BinanceNativeBundle CreateNativeClient(BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateNativeBundle(options);
    }

    /// <summary>
    /// Creates a native bundle using the provided <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The caller-owned HTTP client.</param>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A native bundle for exchange-native Binance calls.</returns>
    public static BinanceNativeBundle CreateNativeClient(HttpClient httpClient, BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateNativeBundle(httpClient, options);
    }
}
