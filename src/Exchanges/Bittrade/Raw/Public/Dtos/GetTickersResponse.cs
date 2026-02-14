using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

public sealed record GetTickersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawTickerEntry>? Data);

public sealed record RawTickerEntry(
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Symbol,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("count")] long Count,
    [property: JsonPropertyName("bid")] decimal? Bid,
    [property: JsonPropertyName("ask")] decimal? Ask);
