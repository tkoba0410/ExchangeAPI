using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Klines;

public sealed class GetKlinesRequest
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("interval")]
    public required string Interval { get; init; }

    [JsonPropertyName("startTime")]
    public string? StartTime { get; init; }

    [JsonPropertyName("endTime")]
    public string? EndTime { get; init; }

    [JsonPropertyName("limit")]
    public int? Limit { get; init; }
}
