using ExchangeApi.Exchanges.Binance.Composition.Options;
using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Binance.LiveTests.Infrastructure;

internal sealed class BinanceLiveTestSettings
{
    private BinanceLiveTestSettings()
    {
    }

    public bool EnableProtocolDebugLogging { get; private init; }
    public string ProtocolDebugLogDirectory { get; private init; } = LiveTestLocalPolicy.LocalPath("logs", "binance", "live-tests");
    public Uri BaseUri { get; private init; } = new("https://api.binance.com");

    public static BinanceLiveTestSettings Load()
    {
        return new BinanceLiveTestSettings
        {
            EnableProtocolDebugLogging = true,
            ProtocolDebugLogDirectory = LiveTestLocalPolicy.LocalPath("logs", "binance", "live-tests"),
            BaseUri = new Uri("https://api.binance.com"),
        };
    }

    public BinanceClientOptions ToClientOptions()
    {
        return new BinanceClientOptions
        {
            BaseUri = BaseUri,
            EnableProtocolDebugLogging = EnableProtocolDebugLogging,
            ProtocolDebugLogDirectory = ProtocolDebugLogDirectory,
        };
    }
}
