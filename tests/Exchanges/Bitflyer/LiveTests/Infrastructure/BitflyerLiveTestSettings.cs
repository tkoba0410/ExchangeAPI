using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Tests.LiveTests.Infrastructure;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.LiveTests.Infrastructure;

internal sealed class BitflyerLiveTestSettings
{
    private BitflyerLiveTestSettings()
    {
    }

    public bool IsLiveEnabled { get; private init; }
    public bool IsWriteEnabled { get; private init; }
    public bool EnableProtocolDebugLogging { get; private init; }
    public string ProtocolDebugLogDirectory { get; private init; } = LiveTestLocalPolicy.LocalPath("logs", "bitflyer", "live-tests");
    public Uri BaseUri { get; private init; } = new("https://api.bitflyer.com");
    public IApiCredentialProvider? ApiCredentialProvider { get; private init; }

    public static BitflyerLiveTestSettings Load()
    {
        var credentials = BitflyerCredentialResolver.Load();

        return new BitflyerLiveTestSettings
        {
            IsLiveEnabled = credentials is not null,
            IsWriteEnabled = BitflyerLiveTestPolicy.HasWriteOptInMarker(),
            EnableProtocolDebugLogging = true,
            ProtocolDebugLogDirectory = LiveTestLocalPolicy.LocalPath("logs", "bitflyer", "live-tests"),
            BaseUri = new Uri("https://api.bitflyer.com"),
            ApiCredentialProvider = credentials,
        };
    }
}
