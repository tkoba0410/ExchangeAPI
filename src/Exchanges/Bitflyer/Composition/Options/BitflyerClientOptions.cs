namespace ExchangeApi.Exchanges.Bitflyer.Composition.Options;

public sealed class BitflyerClientOptions
{
    public Uri BaseUri { get; init; } = new("https://api.bitflyer.com");
    public BitflyerApiCredentials? Credentials { get; init; }
    public bool EnableProtocolDebugLogging { get; init; }
    public string ProtocolDebugLogDirectory { get; init; } = Path.Combine("local", "logs", "bitflyer", "stage10");
}
