using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TradeHistoryResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<TradeHistoryEntry>? Data);

public sealed record TradeHistoryEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(TradeHistoryIdJsonConverter))] TradeHistoryId Id,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<TradeEntry>? Data);
