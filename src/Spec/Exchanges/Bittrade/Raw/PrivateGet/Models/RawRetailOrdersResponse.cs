using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawRetailOrdersResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] IReadOnlyList<RawRetailOrderEntry>? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);

public sealed record RawRetailOrderEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(RetailOrderIdJsonConverter))] string Id,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] string Symbol,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("price")] string? Price,
    [property: JsonPropertyName("amount")] string? Amount,
    [property: JsonPropertyName("cash_amount")] string? CashAmount,
    [property: JsonPropertyName("status")] int? Status,
    [property: JsonPropertyName("created_at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? CreatedAt);
