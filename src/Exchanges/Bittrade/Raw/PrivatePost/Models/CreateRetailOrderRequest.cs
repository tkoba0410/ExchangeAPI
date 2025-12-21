using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CreateRetailOrderRequest(
    [property: JsonPropertyName("symbol")] string Symbol,
    [property: JsonPropertyName("type")] BittradeRetailOrderType Type,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("amount")] string? Amount = null,
    [property: JsonPropertyName("cash_amount")] string? CashAmount = null);

public sealed record RetailOrderResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] long? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);
