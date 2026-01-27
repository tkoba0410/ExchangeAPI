using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

public sealed record RawKlinesResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<RawKlineEntry>? Data);

public sealed record RawKlineEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Id,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("count")] long Count);
