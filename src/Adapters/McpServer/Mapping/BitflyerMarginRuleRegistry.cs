using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerMarginRuleRegistry
{
    private const string ResourceFileName = "bitflyer-margin-rules.v1.json";
    private const string ExpectedVersion = "1";

    private static readonly Lazy<IReadOnlyDictionary<string, BitflyerMarginRule>> Rules = new(LoadRules);

    public static IReadOnlyDictionary<string, BitflyerMarginRule> Entries => Rules.Value;

    public static bool TryGet(string symbol, out BitflyerMarginRule? rule)
    {
        return Rules.Value.TryGetValue(symbol, out rule);
    }

    private static IReadOnlyDictionary<string, BitflyerMarginRule> LoadRules()
    {
        using var stream = OpenResourceStream();
        var file = JsonSerializer.Deserialize<RegistryFile>(stream)
            ?? throw new InvalidOperationException("Bitflyer margin rule data file is empty.");

        if (!string.Equals(file.Version, ExpectedVersion, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Unsupported bitFlyer margin rule data version '{file.Version}'.");
        }

        var rules = new Dictionary<string, BitflyerMarginRule>(StringComparer.Ordinal);
        foreach (var item in file.Rules)
        {
            ValidateRule(item);
            if (!rules.TryAdd(
                    item.Symbol,
                    new BitflyerMarginRule(
                        Symbol: item.Symbol,
                        MinSize: item.MinSize,
                        SizeStep: item.SizeStep,
                        PriceStep: item.PriceStep,
                        MinSizeSourceKind: item.MinSizeSourceKind,
                        MinSizeSourceRef: item.MinSizeSourceRef,
                        SizeStepSourceKind: item.SizeStepSourceKind,
                        SizeStepSourceRef: item.SizeStepSourceRef,
                        PriceStepSourceKind: item.PriceStepSourceKind,
                        PriceStepSourceRef: item.PriceStepSourceRef,
                        RequireCollateralModel: item.RequireCollateralModel,
                        RequireCollateralModelSourceKind: item.RequireCollateralModelSourceKind,
                        RequireCollateralModelSourceRef: item.RequireCollateralModelSourceRef,
                        MaintenanceModel: item.MaintenanceModel,
                        MaintenanceModelSourceKind: item.MaintenanceModelSourceKind,
                        MaintenanceModelSourceRef: item.MaintenanceModelSourceRef,
                        MinimumKeepRate: item.MinimumKeepRate,
                        FeeModel: item.FeeModel,
                        FeeModelSourceKind: item.FeeModelSourceKind,
                        FeeModelSourceRef: item.FeeModelSourceRef,
                        SourceNote: item.SourceNote)))
            {
                throw new InvalidOperationException(
                    $"Duplicate bitFlyer margin rule entry detected for '{item.Symbol}'.");
            }
        }

        return rules;
    }

    private static Stream OpenResourceStream()
    {
        var assembly = typeof(BitflyerMarginRuleRegistry).Assembly;
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
            throw new InvalidOperationException("Bitflyer margin rule symbol must not be empty.");
        }

        ValidateSourceKind(item.Symbol, "minSizeSourceKind", item.MinSizeSourceKind);
        ValidateSourceKind(item.Symbol, "sizeStepSourceKind", item.SizeStepSourceKind);
        ValidateSourceKind(item.Symbol, "priceStepSourceKind", item.PriceStepSourceKind);
        ValidateSourceKind(item.Symbol, "requireCollateralModelSourceKind", item.RequireCollateralModelSourceKind);
        ValidateSourceKind(item.Symbol, "maintenanceModelSourceKind", item.MaintenanceModelSourceKind);
        ValidateSourceKind(item.Symbol, "feeModelSourceKind", item.FeeModelSourceKind);

        ValidateSourceRef(item.Symbol, "minSizeSourceRef", item.MinSizeSourceRef);
        ValidateSourceRef(item.Symbol, "sizeStepSourceRef", item.SizeStepSourceRef);
        ValidateSourceRef(item.Symbol, "priceStepSourceRef", item.PriceStepSourceRef);
        ValidateSourceRef(item.Symbol, "requireCollateralModelSourceRef", item.RequireCollateralModelSourceRef);
        ValidateSourceRef(item.Symbol, "maintenanceModelSourceRef", item.MaintenanceModelSourceRef);
        ValidateSourceRef(item.Symbol, "feeModelSourceRef", item.FeeModelSourceRef);

        if (string.IsNullOrWhiteSpace(item.RequireCollateralModel))
        {
            throw new InvalidOperationException(
                $"requireCollateralModel must not be empty for '{item.Symbol}'.");
        }

        if (string.IsNullOrWhiteSpace(item.MaintenanceModel))
        {
            throw new InvalidOperationException(
                $"maintenanceModel must not be empty for '{item.Symbol}'.");
        }

        if (string.IsNullOrWhiteSpace(item.MinimumKeepRate))
        {
            throw new InvalidOperationException(
                $"minimumKeepRate must not be empty for '{item.Symbol}'.");
        }

        if (string.IsNullOrWhiteSpace(item.FeeModel))
        {
            throw new InvalidOperationException(
                $"feeModel must not be empty for '{item.Symbol}'.");
        }
    }

    private static void ValidateSourceKind(string symbol, string field, string value)
    {
        if (!MarketRuleSourceKinds.IsDefined(value))
        {
            throw new InvalidOperationException(
                $"Unsupported {field} '{value}' for '{symbol}'.");
        }
    }

    private static void ValidateSourceRef(string symbol, string field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{field} must not be empty for '{symbol}'.");
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

        [JsonPropertyName("minSizeSourceRef")]
        public required string MinSizeSourceRef { get; init; }

        [JsonPropertyName("sizeStepSourceKind")]
        public required string SizeStepSourceKind { get; init; }

        [JsonPropertyName("sizeStepSourceRef")]
        public required string SizeStepSourceRef { get; init; }

        [JsonPropertyName("priceStepSourceKind")]
        public required string PriceStepSourceKind { get; init; }

        [JsonPropertyName("priceStepSourceRef")]
        public required string PriceStepSourceRef { get; init; }

        [JsonPropertyName("requireCollateralModel")]
        public required string RequireCollateralModel { get; init; }

        [JsonPropertyName("requireCollateralModelSourceKind")]
        public required string RequireCollateralModelSourceKind { get; init; }

        [JsonPropertyName("requireCollateralModelSourceRef")]
        public required string RequireCollateralModelSourceRef { get; init; }

        [JsonPropertyName("maintenanceModel")]
        public required string MaintenanceModel { get; init; }

        [JsonPropertyName("maintenanceModelSourceKind")]
        public required string MaintenanceModelSourceKind { get; init; }

        [JsonPropertyName("maintenanceModelSourceRef")]
        public required string MaintenanceModelSourceRef { get; init; }

        [JsonPropertyName("minimumKeepRate")]
        public required string MinimumKeepRate { get; init; }

        [JsonPropertyName("feeModel")]
        public required string FeeModel { get; init; }

        [JsonPropertyName("feeModelSourceKind")]
        public required string FeeModelSourceKind { get; init; }

        [JsonPropertyName("feeModelSourceRef")]
        public required string FeeModelSourceRef { get; init; }

        [JsonPropertyName("sourceNote")]
        public required string SourceNote { get; init; }
    }
}
