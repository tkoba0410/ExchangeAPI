using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Tests.Inventory;

namespace ExchangeApi.Docs.Inventory.Tests;

public sealed class InventorySsotAlignmentTests
{
    private static readonly string RepoRoot = InventoryEndpointIdParser.FindRepoRoot();

    [Theory]
    [InlineData(InventoryPaths.BitflyerRelative, typeof(ExchangeApi.Exchanges.Bitflyer.Vocabulary.EndpointIds), "Bitflyer")]
    [InlineData(InventoryPaths.BittradeRelative, typeof(ExchangeApi.Exchanges.Bittrade.Vocabulary.EndpointIds), "Bittrade")]
    public void ExchangeInventory_And_VocabularyEndpointIds_MustMatch(
        string inventoryRelativePath,
        Type endpointIdsType,
        string exchangeName)
    {
        var inventoryPath = ToAbsolute(inventoryRelativePath);
        var inventoryRows = ParseMarkdownTables(inventoryPath)
            .Where(t => t.HasColumns("EndpointId", "PresentIn"))
            .SelectMany(t => t.Rows)
            .ToArray();

        var inventoryIds = inventoryRows
            .Where(r => !IsNoneOrInternal(r.GetValueOrDefault("PresentIn")))
            .Select(r => r.GetValueOrDefault("EndpointId") ?? string.Empty)
            .Where(id => !string.IsNullOrWhiteSpace(id) && !IsNoneOrInternal(id))
            .ToHashSet(StringComparer.Ordinal);

        var vocabularyIds = endpointIdsType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string?)f.GetRawConstantValue())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToHashSet(StringComparer.Ordinal);

        var missing = inventoryIds.Except(vocabularyIds, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        var extra = vocabularyIds.Except(inventoryIds, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0 && extra.Length == 0,
            $"{exchangeName} EndpointId mismatch. Missing: [{string.Join(", ", missing)}], Extra: [{string.Join(", ", extra)}]");
    }

    [Fact]
    public void ContractsInventory_Mappings_MustReference_Existing_ExchangeEndpointIds()
    {
        var bitflyerInventory = LoadExchangeInventory(InventoryPaths.BitflyerRelative);
        var bittradeInventory = LoadExchangeInventory(InventoryPaths.BittradeRelative);

        var bitflyerVocabulary = GetEndpointIds(typeof(ExchangeApi.Exchanges.Bitflyer.Vocabulary.EndpointIds));
        var bittradeVocabulary = GetEndpointIds(typeof(ExchangeApi.Exchanges.Bittrade.Vocabulary.EndpointIds));

        var contractsRows = ParseMarkdownTables(ToAbsolute("docs/inventory/endpoints-contracts.md"))
            .Where(t => t.HasColumns("BitflyerEndpointId", "BittradeEndpointId"))
            .SelectMany(t => t.Rows)
            .ToArray();

        var errors = new List<string>();
        foreach (var row in contractsRows)
        {
            var method = row.GetValueOrDefault("ContractMethod") ?? "(unknown method)";
            ValidateMapping("Bitflyer", method, row.GetValueOrDefault("BitflyerEndpointId"), bitflyerInventory, bitflyerVocabulary, errors);
            ValidateMapping("Bittrade", method, row.GetValueOrDefault("BittradeEndpointId"), bittradeInventory, bittradeVocabulary, errors);
        }

        Assert.True(errors.Count == 0, string.Join(Environment.NewLine, errors));
    }

    [Fact]
    public void ContractsInventory_ContractMethods_MustExist_InFacadeInterfaces()
    {
        var contractMethods = ParseMarkdownTables(ToAbsolute("docs/inventory/endpoints-contracts.md"))
            .Where(t => t.HasColumns("ContractMethod"))
            .SelectMany(t => t.Rows)
            .Select(r => r.GetValueOrDefault("ContractMethod") ?? string.Empty)
            .Where(m => !string.IsNullOrWhiteSpace(m) && !IsNoneOrInternal(m))
            .ToHashSet(StringComparer.Ordinal);

        var facadeInterfaces = new[] { typeof(IPublicApi), typeof(IPrivateApi), typeof(IExchangeMarketResolver), typeof(IExchangeClient) };
        var facadeMethodNames = facadeInterfaces
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .Select(m => m.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missing = contractMethods.Except(facadeMethodNames, StringComparer.Ordinal)
            .OrderBy(x => x, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            $"Contract methods not found in facade interfaces: [{string.Join(", ", missing)}]");
    }

    private static Dictionary<string, string> LoadExchangeInventory(string relativePath)
    {
        return ParseMarkdownTables(ToAbsolute(relativePath))
            .Where(t => t.HasColumns("EndpointId", "PresentIn"))
            .SelectMany(t => t.Rows)
            .Select(r => new
            {
                EndpointId = r.GetValueOrDefault("EndpointId") ?? string.Empty,
                PresentIn = r.GetValueOrDefault("PresentIn") ?? string.Empty,
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.EndpointId) && !IsNoneOrInternal(x.EndpointId) && !IsNoneOrInternal(x.PresentIn))
            .ToDictionary(x => x.EndpointId, x => x.PresentIn, StringComparer.Ordinal);
    }

    private static HashSet<string> GetEndpointIds(Type endpointIdsType)
    {
        return endpointIdsType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string?)f.GetRawConstantValue())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => v!)
            .ToHashSet(StringComparer.Ordinal);
    }

    private static void ValidateMapping(
        string exchange,
        string contractMethod,
        string? endpointId,
        IReadOnlyDictionary<string, string> exchangeInventory,
        IReadOnlySet<string> exchangeVocabulary,
        List<string> errors)
    {
        if (IsNoneOrInternal(endpointId))
        {
            return;
        }

        var id = endpointId!.Trim();

        if (!exchangeInventory.ContainsKey(id))
        {
            errors.Add($"{exchange}: ContractMethod '{contractMethod}' maps to '{id}', but it does not exist in exchange inventory.");
        }

        if (!exchangeVocabulary.Contains(id))
        {
            errors.Add($"{exchange}: ContractMethod '{contractMethod}' maps to '{id}', but it does not exist in Vocabulary.EndpointIds.");
        }
    }

    private static string ToAbsolute(string relativePath)
    {
        return Path.Combine(RepoRoot, relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    private static bool IsNoneOrInternal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        var normalized = value.Trim();
        return string.Equals(normalized, "None", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "Internal", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<MarkdownTable> ParseMarkdownTables(string path)
    {
        var lines = File.ReadAllLines(path);
        var tables = new List<MarkdownTable>();

        for (var i = 0; i < lines.Length; i++)
        {
            if (!lines[i].TrimStart().StartsWith('|', StringComparison.Ordinal))
            {
                continue;
            }

            var header = SplitRow(lines[i]);
            if (header.Count == 0)
            {
                continue;
            }

            if (i + 1 >= lines.Length || !IsSeparatorRow(SplitRow(lines[i + 1])))
            {
                continue;
            }

            var rows = new List<Dictionary<string, string>>();

            for (var j = i + 2; j < lines.Length; j++)
            {
                var rowLine = lines[j];
                if (!rowLine.TrimStart().StartsWith('|', StringComparison.Ordinal))
                {
                    i = j - 1;
                    break;
                }

                var cells = SplitRow(rowLine);
                if (cells.Count == 0 || IsSeparatorRow(cells) || cells.Count < header.Count)
                {
                    continue;
                }

                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var col = 0; col < header.Count; col++)
                {
                    row[header[col]] = cells[col];
                }

                rows.Add(row);
                i = j;
            }

            tables.Add(new MarkdownTable(header, rows));
        }

        return tables;
    }

    private static List<string> SplitRow(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            return new List<string>();
        }

        return line.Trim()
            .Trim('|')
            .Split('|', StringSplitOptions.None)
            .Select(x => x.Trim())
            .ToList();
    }

    private static bool IsSeparatorRow(IReadOnlyList<string> cells)
    {
        return cells.Count > 0 && cells.All(cell => cell.Length > 0 && cell.All(c => c is '-' or ':' or ' '));
    }

    private sealed record MarkdownTable(IReadOnlyList<string> Header, IReadOnlyList<Dictionary<string, string>> Rows)
    {
        public bool HasColumns(params string[] names)
        {
            return names.All(name => Header.Contains(name, StringComparer.OrdinalIgnoreCase));
        }
    }
}
