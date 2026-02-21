using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Common.Tests.Architecture;

public sealed class ExchangeModuleLayoutParityTests
{
    [Fact]
    public void ExchangeModules_ShouldMatchCanonicalDirectoryShape()
    {
        var root = FindRepoRoot();
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
            AssertDirectoryShape(
                exchangePath,
                new[] { "Wire", "Raw", "Normalized", "Adapter", "Composition", "Vocabulary" },
                Array.Empty<string>(),
                scope: $"{exchange} root");

            var wirePath = Path.Combine(exchangePath, "Wire");
            AssertDirectoryShape(
                wirePath,
                new[] { "Public", "Private", "Constants", "Properties", "Internal" },
                Array.Empty<string>(),
                scope: $"{exchange}/Wire");
            AssertDirectoryShape(
                Path.Combine(wirePath, "Public"),
                new[] { "Endpoints" },
                Array.Empty<string>(),
                scope: $"{exchange}/Wire/Public");
            AssertDirectoryShape(
                Path.Combine(wirePath, "Private"),
                new[] { "Endpoints" },
                Array.Empty<string>(),
                scope: $"{exchange}/Wire/Private");

            AssertDirectoryShape(
                Path.Combine(exchangePath, "Raw"),
                new[] { "Public", "Private", "Internal" },
                new[] { "Api" },
                scope: $"{exchange}/Raw");

            AssertDirectoryShape(
                Path.Combine(exchangePath, "Normalized"),
                new[] { "Public", "Private", "Internal" },
                new[] { "Api", "Properties" },
                scope: $"{exchange}/Normalized");

            AssertDirectoryShape(
                Path.Combine(exchangePath, "Adapter"),
                new[] { "Public", "Private", "Bootstrap", "Internal" },
                new[] { "Properties" },
                scope: $"{exchange}/Adapter");

            Assert.True(File.Exists(Path.Combine(exchangePath, "Vocabulary", "EndpointIds.cs")));
            Assert.True(File.Exists(Path.Combine(exchangePath, "Wire", $"ExchangeApi.Exchanges.{exchange}.Wire.csproj")));
            Assert.True(File.Exists(Path.Combine(exchangePath, "Raw", $"ExchangeApi.Exchanges.{exchange}.Raw.csproj")));
            Assert.True(File.Exists(Path.Combine(exchangePath, "Normalized", $"ExchangeApi.Exchanges.{exchange}.Normalized.csproj")));
            Assert.True(File.Exists(Path.Combine(exchangePath, "Adapter", $"ExchangeApi.Exchanges.{exchange}.Adapter.csproj")));
            Assert.True(File.Exists(Path.Combine(exchangePath, "Composition", $"ExchangeApi.Exchanges.{exchange}.Composition.csproj")));
        }
    }

    private static void AssertDirectoryShape(
        string basePath,
        IReadOnlyCollection<string> requiredNames,
        IReadOnlyCollection<string> optionalNames,
        string scope)
    {
        Assert.True(Directory.Exists(basePath), $"Directory not found: {basePath}");

        var children = Directory
            .GetDirectories(basePath)
            .Select(static path => Path.GetFileName(path) ?? string.Empty)
            .Where(static name => name.Length > 0)
            .Where(static name => !IsBuildArtifactDirectory(name))
            .ToHashSet(StringComparer.Ordinal);

        var allowed = requiredNames
            .Concat(optionalNames)
            .ToHashSet(StringComparer.Ordinal);

        var missing = requiredNames
            .Where(name => !children.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var unexpected = children
            .Where(name => !allowed.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            $"Missing required directories in {scope}: {string.Join(", ", missing)}");
        Assert.True(
            unexpected.Length == 0,
            $"Unexpected directories in {scope}: {string.Join(", ", unexpected)}");
    }

    private static bool IsBuildArtifactDirectory(string name) =>
        string.Equals(name, "bin", StringComparison.Ordinal) ||
        string.Equals(name, "obj", StringComparison.Ordinal);

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
