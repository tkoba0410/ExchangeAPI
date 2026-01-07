using System.IO;
using System.Linq;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests.Guard;

public class NoTestFactoryUsageTests
{
    [Fact]
    public void ProductionSrc_ShouldNotReference_TestOnlyTypes()
    {
        var baseDir = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(baseDir);
        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "src")))
        {
            dir = dir.Parent;
        }

        Assert.NotNull(dir);
        var srcRoot = Path.Combine(dir!.FullName, "src");
        var files = Directory.GetFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith("BitflyerTestClientFactory.cs", StringComparison.OrdinalIgnoreCase)
                        && !f.EndsWith("BitflyerApiBundle.cs", StringComparison.OrdinalIgnoreCase)
                        && !f.EndsWith("BitflyerExchangeClient.cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var forbidden = new[]
        {
            "BitflyerTestClientFactory",
            "BitflyerApiBundle"
        };

        var offenders = files
            .Where(f => File.ReadAllText(f).Split('\n').Any(line =>
                forbidden.Any(token => line.Contains(token))))
            .ToArray();

        Assert.True(offenders.Length == 0, $"Forbidden test-only types referenced in: {string.Join(", ", offenders)}");
    }
}
