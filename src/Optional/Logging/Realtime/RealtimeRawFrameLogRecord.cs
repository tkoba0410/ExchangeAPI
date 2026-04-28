using System.Text.Json.Serialization;

namespace ExchangeApi.Optional.Logging.Realtime;

public sealed class RealtimeRawFrameLogRecord
{
    [JsonPropertyName("receivedAt")]
    public required DateTimeOffset ReceivedAt { get; init; }

    [JsonPropertyName("venue")]
    public required string Venue { get; init; }

    [JsonPropertyName("channel")]
    public required string Channel { get; init; }

    [JsonPropertyName("payloadLength")]
    public required int PayloadLength { get; init; }

    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Body { get; init; }

    [JsonPropertyName("bodySkipped")]
    public required bool BodySkipped { get; init; }

    [JsonPropertyName("skipReason")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SkipReason { get; init; }
}
