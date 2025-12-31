using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawTradeEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(TradeIdJsonConverter))] RawTradeId Id,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts);
