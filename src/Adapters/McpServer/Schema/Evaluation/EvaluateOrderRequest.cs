using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Evaluation;

public sealed class EvaluateOrderRequest
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("side")]
    public required string Side { get; init; }

    [JsonPropertyName("orderType")]
    public required string OrderType { get; init; }

    [JsonPropertyName("size")]
    public required string Size { get; init; }

    [JsonPropertyName("price")]
    public string? Price { get; init; }
}
