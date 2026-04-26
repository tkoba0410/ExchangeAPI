using ExchangeApi.Optional.Logging.Evidence;

namespace ExchangeApi.Optional.Logging.Tests;

public sealed class EvidenceRunDirectoryFactoryTests
{
    [Fact]
    public void Create_CreatesStandardDirectoryLayoutAndCollisionSuffix()
    {
        var repoRoot = Path.Combine(Path.GetTempPath(), $"exchangeapi-evidence-{Guid.NewGuid():N}");
        var factory = new EvidenceRunDirectoryFactory(repoRoot, () => new DateTimeOffset(2026, 4, 26, 0, 0, 0, TimeSpan.Zero));

        var first = factory.Create(EvidencePhase.LocalLive, new EvidenceRunLabel("manual run"));
        var second = factory.Create(EvidencePhase.LocalLive, new EvidenceRunLabel("manual run"));

        Assert.EndsWith(Path.Combine("local-live", "20260426-manual-run"), first.Root);
        Assert.EndsWith(Path.Combine("local-live", "20260426-manual-run-2"), second.Root);
        Assert.True(Directory.Exists(first.Artifacts));
        Assert.True(Directory.Exists(first.Logs));
        Assert.True(Directory.Exists(first.Notes));
    }
}
