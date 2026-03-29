using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public sealed class EvaluateOrderResponse
{
    [JsonPropertyName("canPlace")]
    public required bool CanPlace { get; init; }

    [JsonPropertyName("checks")]
    public required EvaluateOrderChecks Checks { get; init; }

    [JsonPropertyName("normalizedRequest")]
    public required EvaluateOrderRequest NormalizedRequest { get; init; }

    [JsonPropertyName("estimate")]
    public required EvaluateOrderEstimate Estimate { get; init; }

    [JsonPropertyName("warnings")]
    public required IReadOnlyList<string> Warnings { get; init; }

    [JsonPropertyName("reasons")]
    public required IReadOnlyList<string> Reasons { get; init; }
}
