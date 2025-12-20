using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record KlinesResponse(
    [property: JsonPropertyName("ch")] string? Channel,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("ts")] long Ts,
    [property: JsonPropertyName("data")] IReadOnlyList<KlineEntry>? Data);

public sealed record KlineEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("count")] long Count);
