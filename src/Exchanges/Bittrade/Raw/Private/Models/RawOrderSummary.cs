using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawOrderSummary(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Id,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Symbol,
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("client-order-id")] string? ClientOrderId,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("field-amount")] string FilledAmount);
