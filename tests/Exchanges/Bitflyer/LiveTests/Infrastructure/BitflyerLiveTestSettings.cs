using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerLiveTestSettings
{
    private BitflyerLiveTestSettings()
    {
    }

    public bool IsLiveEnabled { get; private init; }
    public bool IsWriteEnabled { get; private init; }
    public bool EnableProtocolDebugLogging { get; private init; }
    public Uri BaseUri { get; private init; } = new("https://api.bitflyer.com");
    public BitflyerApiCredentials? Credentials { get; private init; }

    public static BitflyerLiveTestSettings Load()
    {
        var credentials = BitflyerCredentialResolver.Load();

        return new BitflyerLiveTestSettings
        {
            IsLiveEnabled = credentials is not null,
            IsWriteEnabled = BitflyerLiveTestPolicy.HasWriteOptInMarker(),
            EnableProtocolDebugLogging = false,
            BaseUri = new Uri("https://api.bitflyer.com"),
            Credentials = credentials,
        };
    }
}
