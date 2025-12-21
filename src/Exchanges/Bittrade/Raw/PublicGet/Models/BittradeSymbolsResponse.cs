using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record SymbolsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<SymbolInfo>? Data);

public sealed record SymbolInfo(
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] Symbol Symbol,
    [property: JsonPropertyName("base-currency")] string BaseCurrency,
    [property: JsonPropertyName("quote-currency")] string QuoteCurrency,
    [property: JsonPropertyName("price-precision")] int PricePrecision,
    [property: JsonPropertyName("amount-precision")] int AmountPrecision,
    [property: JsonPropertyName("value-precision")] int? ValuePrecision,
    [property: JsonPropertyName("min-order-amt")] JsonElement MinOrderAmount,
    [property: JsonPropertyName("min-order-value")] JsonElement MinOrderValue,
    [property: JsonPropertyName("state")] string State);
