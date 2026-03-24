using ExchangeApi.Exchanges.Binance.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Binance.LiveTests.Infrastructure;

internal sealed class BinanceLiveTestSettings
{
    private BinanceLiveTestSettings()
    {
    }

    public bool EnableProtocolDebugLogging { get; private init; }
    public Uri BaseUri { get; private init; } = new("https://api.binance.com");

    public static BinanceLiveTestSettings Load()
    {
        return new BinanceLiveTestSettings
        {
            EnableProtocolDebugLogging = false,
            BaseUri = new Uri("https://api.binance.com"),
        };
    }

    public BinanceClientOptions ToClientOptions()
    {
        return new BinanceClientOptions
        {
            BaseUri = BaseUri,
            EnableProtocolDebugLogging = EnableProtocolDebugLogging,
            ProtocolDebugLogDirectory = Path.Combine("local", "logs", "binance", "stage10", "live-tests"),
        };
    }
}
