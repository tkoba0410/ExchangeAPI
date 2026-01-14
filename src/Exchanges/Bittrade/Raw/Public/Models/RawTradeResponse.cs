using System;
using System.Text.Json.Serialization;
using ExchangeApi.Contracts.Common.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record RawTradeResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] RawTradeTick? Tick,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts);
