using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;

public sealed class EvaluateMarginOrderChecks
{
    [JsonPropertyName("symbolOk")]
    public required bool SymbolOk { get; init; }

    [JsonPropertyName("marketStatusOk")]
    public required bool MarketStatusOk { get; init; }

    [JsonPropertyName("sizeRuleOk")]
    public required bool SizeRuleOk { get; init; }

    [JsonPropertyName("priceRuleOk")]
    public required bool PriceRuleOk { get; init; }

    [JsonPropertyName("collateralCoverageOk")]
    public required bool CollateralCoverageOk { get; init; }

    [JsonPropertyName("feeCoverageOk")]
    public required bool? FeeCoverageOk { get; init; }

    [JsonPropertyName("projectedMarginExposureOk")]
    public required bool ProjectedMarginExposureOk { get; init; }

    [JsonPropertyName("currentMaintenanceOk")]
    public required bool CurrentMaintenanceOk { get; init; }
}
