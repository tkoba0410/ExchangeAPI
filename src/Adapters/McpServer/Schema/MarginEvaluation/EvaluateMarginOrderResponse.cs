using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;

public sealed class EvaluateMarginOrderResponse
{
    [JsonPropertyName("canPlace")]
    public required bool CanPlace { get; init; }

    [JsonPropertyName("checks")]
    public required EvaluateMarginOrderChecks Checks { get; init; }

    [JsonPropertyName("normalizedRequest")]
    public required EvaluateMarginOrderRequest NormalizedRequest { get; init; }

    [JsonPropertyName("estimate")]
    public required EvaluateMarginOrderEstimate Estimate { get; init; }

    [JsonPropertyName("warnings")]
    public required IReadOnlyList<string> Warnings { get; init; }

    [JsonPropertyName("reasons")]
    public required IReadOnlyList<string> Reasons { get; init; }
}
