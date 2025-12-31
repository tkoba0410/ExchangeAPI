using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawMergedResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] RawMergedTick? Tick,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts);
