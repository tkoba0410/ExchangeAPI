using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;

public sealed class EvaluateMarginOrderEstimate
{
    [JsonPropertyName("referencePrice")]
    public required string ReferencePrice { get; init; }

    [JsonPropertyName("estimatedNotional")]
    public required string EstimatedNotional { get; init; }

    [JsonPropertyName("estimatedRequiredCollateral")]
    public required string EstimatedRequiredCollateral { get; init; }

    [JsonPropertyName("currentMaxLeverage")]
    public required string CurrentMaxLeverage { get; init; }

    [JsonPropertyName("currentKeepRate")]
    public required string CurrentKeepRate { get; init; }

    [JsonPropertyName("minimumKeepRate")]
    public required string MinimumKeepRate { get; init; }

    [JsonPropertyName("estimatedFee")]
    public required string? EstimatedFee { get; init; }

    [JsonPropertyName("estimatedFeeSourceKind")]
    public required string? EstimatedFeeSourceKind { get; init; }
}
