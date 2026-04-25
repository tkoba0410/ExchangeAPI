using ExchangeApi.Primitives.Credentials;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Options;

public sealed class BitflyerClientOptions
{
    public Uri BaseUri { get; init; } = new("https://api.bitflyer.com");
    public TimeSpan? RequestTimeout { get; init; }
    public IApiCredentialProvider? ApiCredentialProvider { get; init; }
    public bool UseTickerAliasPath { get; init; }
    public bool EnableProtocolDebugLogging { get; init; }
    public string ProtocolDebugLogDirectory { get; init; } = Path.Combine("local", "logs", "bitflyer", "protocol");
}
