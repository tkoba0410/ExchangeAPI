namespace ExchangeApi.Exchanges.Binance.Composition.Options;

public sealed class BinanceClientOptions
{
    public Uri BaseUri { get; init; } = new("https://api.binance.com");
    public bool EnableProtocolDebugLogging { get; init; }
    public string ProtocolDebugLogDirectory { get; init; } = Path.Combine("local", "logs", "binance", "protocol");
}
