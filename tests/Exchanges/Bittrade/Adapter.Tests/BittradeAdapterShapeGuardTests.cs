using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests;

public sealed class BittradeAdapterShapeGuardTests
{
    [Fact]
    public void AdapterApi_DoesNotContainOperationLiterals()
    {
        var root = FindRepoRoot();
        var adapterPath = Path.Combine(root, "src", "Exchanges", "Bittrade", "Adapter", "Api");
        var operationsFile = Path.Combine(adapterPath, "Operations", "BittradeOperations.cs");

        var files = Directory.GetFiles(adapterPath, "*.cs", SearchOption.AllDirectories)
            .Where(path => !PathEquals(path, operationsFile))
            .ToArray();

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);
            Assert.DoesNotContain("\"Bittrade.", text, StringComparison.Ordinal);
        }
    }

    [Fact]
    public void MarketDataApi_DoesNotContainSymbolStringTransforms()
    {
        var root = FindRepoRoot();
        var marketApiPath = Path.Combine(
            root,
            "src",
            "Exchanges",
            "Bittrade",
            "Adapter",
            "Api",
            "Market",
            "BittradeMarketApi.cs");

        var text = File.ReadAllText(marketApiPath);
        Assert.DoesNotContain("Replace(\"_\"", text, StringComparison.Ordinal);
        Assert.DoesNotContain("ToLowerInvariant()", text, StringComparison.Ordinal);
    }

    private static bool PathEquals(string left, string right) =>
        string.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.OrdinalIgnoreCase);

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

        throw new DirectoryNotFoundException("Repository root not found (ExchangeApi.slnx missing)." );
    }
}
