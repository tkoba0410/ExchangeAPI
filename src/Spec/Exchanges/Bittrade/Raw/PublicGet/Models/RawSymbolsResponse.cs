using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawSymbolsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawSymbolInfo>? Data);

public sealed record RawSymbolInfo(
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Symbol,
    [property: JsonPropertyName("base-currency")] string BaseCurrency,
    [property: JsonPropertyName("quote-currency")] string QuoteCurrency,
    [property: JsonPropertyName("price-precision")] int PricePrecision,
    [property: JsonPropertyName("amount-precision")] int AmountPrecision,
    [property: JsonPropertyName("value-precision")] int? ValuePrecision,
    [property: JsonPropertyName("min-order-amt")] JsonElement MinOrderAmount,
    [property: JsonPropertyName("min-order-value")] JsonElement MinOrderValue,
    [property: JsonPropertyName("state")] string State);
