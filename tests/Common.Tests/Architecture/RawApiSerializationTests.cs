using System;
using System.IO;
using System.Linq;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests;

public sealed class RawApiSerializationTests
{
    [Fact]
    public void RawApi_DoesNotUseSerializeOrThrowOutsideRawJson()
    {
        var root = FindRepoRoot();
        var rawApiDirs = new[]
        {
            Path.Combine(root, "src", "Exchanges", "Bitflyer", "Raw"),
            Path.Combine(root, "src", "Exchanges", "Bittrade", "Raw"),
        };

        var offenders = rawApiDirs
            .Where(Directory.Exists)
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
            .Where(path => !path.EndsWith("RawJson.cs", StringComparison.Ordinal))
            .Where(path => File.ReadAllText(path).Contains("SerializeOrThrow(", StringComparison.Ordinal))
            .ToArray();

        Assert.True(offenders.Length == 0, $"SerializeOrThrow usage found: {string.Join(", ", offenders)}");
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "ExchangeApi.slnx")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            throw new InvalidOperationException("Repository root not found.");
        }

        return dir.FullName;
    }
}
