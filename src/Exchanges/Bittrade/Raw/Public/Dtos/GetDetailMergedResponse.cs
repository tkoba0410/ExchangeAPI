using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

public sealed record GetDetailMergedResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] GetDetailMergedTick? Tick,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts);
