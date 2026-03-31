using ExchangeApi.Exchanges.Bitflyer.Composition.Bootstrap;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

/// <summary>
/// Creates bitFlyer protocol and native client bundles from the Composition layer.
/// </summary>
public static class BitflyerClientFactory
{
    /// <summary>
    /// Creates a protocol bundle with an internally managed <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A protocol bundle for raw/debug-oriented bitFlyer calls.</returns>
    public static BitflyerProtocolBundle CreateProtocolClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateProtocolBundle(options);
    }

    /// <summary>
    /// Creates a protocol bundle using the provided <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The caller-owned HTTP client.</param>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A protocol bundle for raw/debug-oriented bitFlyer calls.</returns>
    public static BitflyerProtocolBundle CreateProtocolClient(HttpClient httpClient, BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateProtocolBundle(httpClient, options);
    }

    /// <summary>
    /// Creates a native bundle with an internally managed <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A native bundle for exchange-native bitFlyer calls.</returns>
    public static BitflyerNativeBundle CreateNativeClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateNativeBundle(options);
    }

    /// <summary>
    /// Creates a native bundle using the provided <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">The caller-owned HTTP client.</param>
    /// <param name="options">Optional client options. Null uses defaults.</param>
    /// <returns>A native bundle for exchange-native bitFlyer calls.</returns>
    public static BitflyerNativeBundle CreateNativeClient(HttpClient httpClient, BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateNativeBundle(httpClient, options);
    }
}
