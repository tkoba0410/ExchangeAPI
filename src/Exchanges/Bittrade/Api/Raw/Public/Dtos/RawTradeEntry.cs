using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;

public sealed record RawTradeEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Id,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts);
