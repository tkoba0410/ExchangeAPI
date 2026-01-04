using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawTradeHistoryResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeHistoryEntry>? Data);

public sealed record RawTradeHistoryEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(TradeHistoryIdJsonConverter))] string Id,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeEntry>? Data);
