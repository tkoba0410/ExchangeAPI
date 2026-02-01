using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;

public sealed record RawMergedTick(
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts,
    [property: JsonPropertyName("bid")] decimal[] Bid,
    [property: JsonPropertyName("ask")] decimal[] Ask);
