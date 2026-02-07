using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

public sealed record RawTimestampResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Data);
