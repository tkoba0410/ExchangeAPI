using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TradeHistoryResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")] long Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<TradeHistoryEntry>? Data);

public sealed record TradeHistoryEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("ts")] long Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<TradeEntry>? Data);
