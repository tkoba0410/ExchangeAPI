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
        var baseUriText = Environment.GetEnvironmentVariable("BITFLYER_API_BASE_URI");
        var apiKey = Environment.GetEnvironmentVariable("BITFLYER_API_KEY");
        var apiSecret = Environment.GetEnvironmentVariable("BITFLYER_API_SECRET");

        return new BitflyerLiveTestSettings
        {
            IsLiveEnabled = Environment.GetEnvironmentVariable("BITFLYER_STAGE10_LIVE") == "1",
            IsWriteEnabled = Environment.GetEnvironmentVariable("BITFLYER_STAGE10_ALLOW_WRITE") == "1",
            EnableProtocolDebugLogging = Environment.GetEnvironmentVariable("BITFLYER_STAGE10_DEBUG_LOG") == "1",
            BaseUri = string.IsNullOrWhiteSpace(baseUriText)
                ? new Uri("https://api.bitflyer.com")
                : new Uri(baseUriText, UriKind.Absolute),
            Credentials = string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret)
                ? null
                : new BitflyerApiCredentials
                {
                    ApiKey = apiKey,
                    ApiSecret = apiSecret,
                },
        };
    }
}
