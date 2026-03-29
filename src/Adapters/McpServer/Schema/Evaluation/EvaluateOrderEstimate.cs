using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public sealed class EvaluateOrderEstimate
{
    [JsonPropertyName("referencePrice")]
    public required string ReferencePrice { get; init; }

    [JsonPropertyName("estimatedNotional")]
    public required string EstimatedNotional { get; init; }
}
