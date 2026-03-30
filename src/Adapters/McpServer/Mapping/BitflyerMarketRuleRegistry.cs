using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerMarketRuleRegistry
{
    private const string ResourceFileName = "bitflyer-market-rules.v1.json";
    private const string ExpectedVersion = "1";

    private static readonly Lazy<IReadOnlyDictionary<string, BitflyerMarketRule>> Rules = new(LoadRules);

    public static IReadOnlyDictionary<string, BitflyerMarketRule> Entries => Rules.Value;

    public static bool TryGet(string symbol, out BitflyerMarketRule? rule)
    {
        return Rules.Value.TryGetValue(symbol, out rule);
    }

    private static IReadOnlyDictionary<string, BitflyerMarketRule> LoadRules()
    {
        using var stream = OpenResourceStream();
        var file = JsonSerializer.Deserialize<RegistryFile>(stream)
            ?? throw new InvalidOperationException("Bitflyer market rule data file is empty.");

        if (!string.Equals(file.Version, ExpectedVersion, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Unsupported bitFlyer market rule data version '{file.Version}'.");
        }

        var rules = new Dictionary<string, BitflyerMarketRule>(StringComparer.Ordinal);
        foreach (var item in file.Rules)
        {
            ValidateRule(item);
            if (!rules.TryAdd(
                    item.Symbol,
                    new BitflyerMarketRule(
                        Symbol: item.Symbol,
                        MinSize: item.MinSize,
                        SizeStep: item.SizeStep,
                        PriceStep: item.PriceStep,
                        MinSizeSourceKind: item.MinSizeSourceKind,
                        SizeStepSourceKind: item.SizeStepSourceKind,
                        PriceStepSourceKind: item.PriceStepSourceKind,
                        SourceNote: item.SourceNote)))
            {
                throw new InvalidOperationException(
                    $"Duplicate bitFlyer market rule entry detected for '{item.Symbol}'.");
            }
        }

        return rules;
    }

    private static Stream OpenResourceStream()
    {
        var assembly = typeof(BitflyerMarketRuleRegistry).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .SingleOrDefault(name => name.EndsWith(ResourceFileName, StringComparison.Ordinal));

        if (resourceName is null)
        {
            throw new InvalidOperationException(
                $"Embedded resource '{ResourceFileName}' was not found.");
        }

        return assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' could not be opened.");
    }

    private static void ValidateRule(RegistryRule item)
    {
        if (string.IsNullOrWhiteSpace(item.Symbol))
        {
            throw new InvalidOperationException("Bitflyer market rule symbol must not be empty.");
        }

        if (!MarketRuleSourceKinds.IsDefined(item.MinSizeSourceKind))
        {
            throw new InvalidOperationException(
                $"Unsupported minSizeSourceKind '{item.MinSizeSourceKind}' for '{item.Symbol}'.");
        }

        if (!MarketRuleSourceKinds.IsDefined(item.SizeStepSourceKind))
        {
            throw new InvalidOperationException(
                $"Unsupported sizeStepSourceKind '{item.SizeStepSourceKind}' for '{item.Symbol}'.");
        }

        if (!MarketRuleSourceKinds.IsDefined(item.PriceStepSourceKind))
        {
            throw new InvalidOperationException(
                $"Unsupported priceStepSourceKind '{item.PriceStepSourceKind}' for '{item.Symbol}'.");
        }
    }

    private sealed class RegistryFile
    {
        [JsonPropertyName("version")]
        public required string Version { get; init; }

        [JsonPropertyName("rules")]
        public required IReadOnlyList<RegistryRule> Rules { get; init; }
    }

    private sealed class RegistryRule
    {
        [JsonPropertyName("symbol")]
        public required string Symbol { get; init; }

        [JsonPropertyName("minSize")]
        public required string MinSize { get; init; }

        [JsonPropertyName("sizeStep")]
        public required string SizeStep { get; init; }

        [JsonPropertyName("priceStep")]
        public required string PriceStep { get; init; }

        [JsonPropertyName("minSizeSourceKind")]
        public required string MinSizeSourceKind { get; init; }

        [JsonPropertyName("sizeStepSourceKind")]
        public required string SizeStepSourceKind { get; init; }

        [JsonPropertyName("priceStepSourceKind")]
        public required string PriceStepSourceKind { get; init; }

        [JsonPropertyName("sourceNote")]
        public required string SourceNote { get; init; }
    }
}
