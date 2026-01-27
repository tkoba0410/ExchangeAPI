using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

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
    [property: JsonPropertyName("min-order-amt")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string MinOrderAmount,
    [property: JsonPropertyName("min-order-value")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string? MinOrderValue,
    [property: JsonPropertyName("state")] string State);
