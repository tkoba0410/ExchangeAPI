using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCreateRetailOrderRequest(
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] RawSymbol RawSymbol,
    [property: JsonPropertyName("type")] RetailOrderType Type,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("amount")] string? Amount = null,
    [property: JsonPropertyName("cash_amount")] string? CashAmount = null);

public sealed record RawRetailOrderResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] long? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);
