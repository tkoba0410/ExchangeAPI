using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record RawTradeHistoryResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeHistoryEntry>? Data);

public sealed record RawTradeHistoryEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Id,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeEntry>? Data);
