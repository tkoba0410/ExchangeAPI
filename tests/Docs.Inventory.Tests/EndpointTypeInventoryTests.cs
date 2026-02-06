using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace ExchangeApi.Docs.Inventory.Tests;

public sealed class EndpointTypeInventoryTests
{
    private readonly ITestOutputHelper _output;

    public EndpointTypeInventoryTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData("docs/inventory/endpoints-bittrade.md")]
    [InlineData("docs/inventory/endpoints-bitflyer.md")]
    public void Inventory_Request_Response_Types_Should_Exist(string relativePath)
    {
        var repoRoot = FindRepoRoot();
        var path = Path.Combine(repoRoot.FullName, relativePath.Replace('/', Path.DirectorySeparatorChar));
        Assert.True(File.Exists(path), $"Inventory file not found: {path}");

        var lines = File.ReadAllLines(path);
        var tables = ParseEndpointTables(lines);
        Assert.NotEmpty(tables);

        var typeIndex = BuildTypeIndex();

        foreach (var table in tables)
        {
            Assert.Contains("RequestType", table.Header);
            Assert.Contains("ResponseType", table.Header);

            foreach (var row in table.Rows)
            {
                var endpointId = row.GetValueOrDefault("EndpointId") ?? "";
                var requestType = row.GetValueOrDefault("RequestType") ?? "";
                var responseType = row.GetValueOrDefault("ResponseType") ?? "";

                Assert.False(string.IsNullOrWhiteSpace(endpointId), "EndpointId must not be empty.");
                Assert.False(string.IsNullOrWhiteSpace(requestType), $"RequestType is empty for EndpointId '{endpointId}'.");
                Assert.False(string.IsNullOrWhiteSpace(responseType), $"ResponseType is empty for EndpointId '{endpointId}'.");

                if (!IsNone(requestType))
                {
                    AssertTypeExists(typeIndex, requestType, $"RequestType for EndpointId '{endpointId}'");
                }

                if (!IsNone(responseType))
                {
                    AssertTypeExists(typeIndex, responseType, $"ResponseType for EndpointId '{endpointId}'");
                }
            }
        }
    }

    private static DirectoryInfo FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "docs", "inventory")))
        {
            dir = dir.Parent;
        }

        if (dir is null)
        {
            throw new DirectoryNotFoundException("Repository root not found (missing docs/inventory). ");
        }

        return dir;
    }

    private static IReadOnlyList<InventoryTable> ParseEndpointTables(string[] lines)
    {
        var tables = new List<InventoryTable>();
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (!line.TrimStart().StartsWith('|'))
            {
                continue;
            }

            var header = SplitRow(line);
            if (!header.Contains("EndpointId", StringComparer.Ordinal) ||
                !header.Contains("PresentIn", StringComparer.Ordinal))
            {
                continue;
            }

            var separatorIndex = i + 1;
            if (separatorIndex >= lines.Length)
            {
                continue;
            }

            var rows = new List<Dictionary<string, string>>();
            i = separatorIndex;

            for (var j = i + 1; j < lines.Length; j++)
            {
                var rowLine = lines[j];
                if (!rowLine.TrimStart().StartsWith('|'))
                {
                    i = j - 1;
                    break;
                }

                if (IsSeparatorRow(rowLine))
                {
                    continue;
                }

                var row = SplitRow(rowLine);
                if (row.Count < header.Count)
                {
                    continue;
                }

                var dict = new Dictionary<string, string>(StringComparer.Ordinal);
                for (var k = 0; k < header.Count; k++)
                {
                    dict[header[k]] = row[k];
                }

                rows.Add(dict);
                i = j;
            }

            tables.Add(new InventoryTable(header, rows));
        }

        return tables;
    }

    private static List<string> SplitRow(string line)
    {
        return line.Trim().Trim('|')
            .Split('|', StringSplitOptions.None)
            .Select(cell => cell.Trim())
            .ToList();
    }

    private static bool IsSeparatorRow(string line)
    {
        var trimmed = line.Trim().Trim('|');
        return trimmed.Length > 0 && trimmed.All(c => c == '-' || c == ' ');
    }

    private static Dictionary<string, List<Type>> BuildTypeIndex()
    {
        var types = new List<Type>();
        var assemblies = new List<Assembly>();

        assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());

        foreach (var dll in Directory.EnumerateFiles(AppContext.BaseDirectory, "*.dll"))
        {
            try
            {
                var loaded = Assembly.LoadFrom(dll);
                if (assemblies.All(a => a.FullName != loaded.FullName))
                {
                    assemblies.Add(loaded);
                }
            }
            catch
            {
                // Ignore load failures for unrelated assemblies.
            }
        }

        foreach (var assembly in assemblies)
        {
            try
            {
                types.AddRange(assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException ex)
            {
                types.AddRange(ex.Types.Where(t => t is not null)!.Select(t => t!));
            }
        }

        var index = new Dictionary<string, List<Type>>(StringComparer.Ordinal);
        foreach (var type in types)
        {
            if (!index.TryGetValue(type.Name, out var list))
            {
                list = new List<Type>();
                index[type.Name] = list;
            }

            list.Add(type);
        }

        return index;
    }

    private void AssertTypeExists(Dictionary<string, List<Type>> index, string typeName, string context)
    {
        if (!index.TryGetValue(typeName, out var matches) || matches.Count == 0)
        {
            Assert.Fail($"{context}: type '{typeName}' was not found.");
        }

        if (matches.Count > 1)
        {
            var fullNames = matches.Select(t => t.FullName).OrderBy(name => name, StringComparer.Ordinal);
            _output.WriteLine($"Ambiguous type name '{typeName}': {string.Join(", ", fullNames)}");
        }
    }

    private static bool IsNone(string value) =>
        string.Equals(value, "None", StringComparison.Ordinal);

    private sealed record InventoryTable(IReadOnlyList<string> Header, IReadOnlyList<Dictionary<string, string>> Rows);
}
