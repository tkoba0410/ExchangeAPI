using System;
using System.Text.Json.Serialization;
using ExchangeApi.Contracts.Common.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record RawMergedResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] RawMergedTick? Tick,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts);
