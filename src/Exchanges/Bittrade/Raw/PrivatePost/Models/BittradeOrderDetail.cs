using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record OrderDetail(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("state")] BittradeOrderState State,
    [property: JsonPropertyName("type")] BittradeOrderType Type,
    [property: JsonPropertyName("client-order-id")] string? ClientOrderId,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("finished-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? FinishedAt,
    [property: JsonPropertyName("field-amount")] string FilledAmount,
    [property: JsonPropertyName("field-cash-amount")] string FilledCashAmount,
    [property: JsonPropertyName("field-fees")] string Fees);
