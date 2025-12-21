using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TradeEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(TradeIdJsonConverter))] TradeId Id,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts);
