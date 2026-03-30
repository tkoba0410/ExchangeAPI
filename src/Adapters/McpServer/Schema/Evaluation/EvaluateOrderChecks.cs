using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public sealed class EvaluateOrderChecks
{
    [JsonPropertyName("symbolOk")]
    public required bool SymbolOk { get; init; }

    [JsonPropertyName("marketStatusOk")]
    public required bool MarketStatusOk { get; init; }

    [JsonPropertyName("sizeRuleOk")]
    public required bool SizeRuleOk { get; init; }

    [JsonPropertyName("priceRuleOk")]
    public required bool PriceRuleOk { get; init; }

    [JsonPropertyName("balanceOk")]
    public required bool BalanceOk { get; init; }

    [JsonPropertyName("projectedExposureOk")]
    public required bool ProjectedExposureOk { get; init; }
}
