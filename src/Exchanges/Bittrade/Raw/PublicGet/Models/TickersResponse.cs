using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TickersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<TickerEntry>? Data);

public sealed record TickerEntry(
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] Symbol Symbol,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("count")] long Count,
    [property: JsonPropertyName("bid")] decimal? Bid,
    [property: JsonPropertyName("ask")] decimal? Ask);
