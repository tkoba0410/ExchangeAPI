using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record OrderSummary(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(OrderIdJsonConverter))] OrderId Id,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] Symbol Symbol,
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("state")] OrderState State,
    [property: JsonPropertyName("type")] OrderType Type,
    [property: JsonPropertyName("client-order-id")] string? ClientOrderId,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("field-amount")] string FilledAmount);
