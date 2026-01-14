using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Contracts.Common.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record RawDepthTick(
    [property: JsonPropertyName("bids")] IReadOnlyList<IReadOnlyList<decimal>>? Bids,
    [property: JsonPropertyName("asks")] IReadOnlyList<IReadOnlyList<decimal>>? Asks,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? Ts);
