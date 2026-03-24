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
        var baseUriText = Environment.GetEnvironmentVariable("BINANCE_API_BASE_URI");

        return new BinanceLiveTestSettings
        {
            EnableProtocolDebugLogging = Environment.GetEnvironmentVariable("BINANCE_STAGE10_DEBUG_LOG") == "1",
            BaseUri = string.IsNullOrWhiteSpace(baseUriText)
                ? new Uri("https://api.binance.com")
                : new Uri(baseUriText, UriKind.Absolute),
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
