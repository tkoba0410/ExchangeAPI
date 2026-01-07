using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCreateOrderRequest(
    [property: JsonPropertyName("account-id")] string AccountId,
    [property: JsonPropertyName("symbol")]
    string Symbol,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("source")] string? Source = null);
