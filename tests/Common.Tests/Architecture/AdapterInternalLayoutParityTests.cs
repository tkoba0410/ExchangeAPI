using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Common.Tests.Architecture;

public sealed class AdapterInternalLayoutParityTests
{
    [Fact]
    public void AdapterInternalLayout_ShouldMatchCanonicalPhaseStructure()
    {
        var root = FindRepoRoot();
        var shape = ExchangeModuleLayoutShape.Load(root);
        var exchangesRoot = Path.Combine(root, "src", "Exchanges");
        Assert.True(Directory.Exists(exchangesRoot));

        var exchanges = Directory
            .GetDirectories(exchangesRoot)
            .Select(static path => Path.GetFileName(path) ?? string.Empty)
            .Where(static name => name.Length > 0)
            .Where(static name => !string.Equals(name, "Common", StringComparison.Ordinal))
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.NotEmpty(exchanges);

        foreach (var exchange in exchanges)
        {
            var exchangePath = Path.Combine(exchangesRoot, exchange);
            foreach (var template in shape.AdapterRequiredFiles)
            {
                var requiredFilePath = Path.Combine(
                    exchangePath,
                    ExchangeModuleLayoutShape.ExpandPathTemplate(template, exchange));
                Assert.True(File.Exists(requiredFilePath), $"Missing adapter required file: {requiredFilePath}");
            }

            var mapPatternPath = ExchangeModuleLayoutShape.ExpandPathTemplate(shape.AdapterMapPattern, exchange);
            var mapDirectoryPath = Path.Combine(exchangePath, Path.GetDirectoryName(mapPatternPath) ?? string.Empty);
            Assert.True(Directory.Exists(mapDirectoryPath), $"Directory not found: {mapDirectoryPath}");

            var mapperFiles = Directory
                .GetFiles(mapDirectoryPath, Path.GetFileName(mapPatternPath), SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(static name => !string.IsNullOrEmpty(name))
                .ToArray();
            Assert.NotEmpty(mapperFiles);
        }
    }

    private static string FindRepoRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "ExchangeApi.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Repository root not found (ExchangeApi.slnx missing).");
    }
}
