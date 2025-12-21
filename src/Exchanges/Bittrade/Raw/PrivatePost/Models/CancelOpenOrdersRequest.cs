using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CancelOpenOrdersRequest(
    [property: JsonPropertyName("account-id")] string? AccountId = null,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] Symbol? Symbol = null,
    [property: JsonPropertyName("side")] OrderSide? Side = null,
    [property: JsonPropertyName("size")] string? Size = null,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? CreatedAt = null);
