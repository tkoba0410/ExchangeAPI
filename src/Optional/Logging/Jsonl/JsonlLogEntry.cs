using System.Text.Json.Serialization;

namespace ExchangeApi.Optional.Logging.Jsonl;

public sealed class JsonlLogEntry
{
    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("level")]
    public required string Level { get; init; }

    [JsonPropertyName("category")]
    public required string Category { get; init; }

    [JsonPropertyName("eventName")]
    public required string EventName { get; init; }

    [JsonPropertyName("data")]
    public object? Data { get; init; }
}
