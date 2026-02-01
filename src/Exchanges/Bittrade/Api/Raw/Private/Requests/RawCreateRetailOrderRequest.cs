using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawCreateRetailOrderRequest(
    [property: JsonPropertyName("symbol")]
    string Symbol,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("amount")] string? Amount = null,
    [property: JsonPropertyName("cash_amount")] string? CashAmount = null);
