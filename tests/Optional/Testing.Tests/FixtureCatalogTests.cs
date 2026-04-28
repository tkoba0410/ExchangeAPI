namespace ExchangeApi.Optional.Testing.Tests;

public sealed class FixtureCatalogTests
{
    [Fact]
    public void RealtimeRawFrameFixtures_DoNotContainSecretMarkers()
    {
        var fixtureDirectory = Path.Combine(
            AppContext.BaseDirectory,
            "Fixtures",
            "Realtime",
            "Bitflyer",
            "RawFrames");

        var fixturePaths = Directory.GetFiles(fixtureDirectory, "*.json", SearchOption.AllDirectories);
        Assert.NotEmpty(fixturePaths);

        var secretMarkers = new[]
        {
            "api_key",
            "apiKey",
            "api-secret",
            "api_secret",
            "apiSecret",
            "signature",
            "Authorization",
            "auth",
        };

        foreach (var fixturePath in fixturePaths)
        {
            var text = File.ReadAllText(fixturePath);
            foreach (var marker in secretMarkers)
            {
                Assert.DoesNotContain(marker, text, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
