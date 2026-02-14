using System.IO;
using System.Linq;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Guard;

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
        var srcRoot = Path.Combine(dir!.FullName, "src", "Exchanges", "Bitflyer", "Adapter");
        var files = Directory.GetFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.EndsWith("TestClientFactory.cs", StringComparison.OrdinalIgnoreCase)
                        && !f.EndsWith("ApiBundle.cs", StringComparison.OrdinalIgnoreCase)
                        && !f.EndsWith("ExchangeClient.cs", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var forbidden = new[]
        {
            "TestClientFactory",
            "ApiBundle"
        };

        var offenders = files
            .Where(f => File.ReadAllText(f).Split('\n').Any(line =>
                forbidden.Any(token => line.Contains(token))))
            .ToArray();

        Assert.True(offenders.Length == 0, $"Forbidden test-only types referenced in: {string.Join(", ", offenders)}");
    }
}
