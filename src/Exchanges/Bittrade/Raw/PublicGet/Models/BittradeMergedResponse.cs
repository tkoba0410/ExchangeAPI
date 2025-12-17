using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeMergedResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeMergedTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);
