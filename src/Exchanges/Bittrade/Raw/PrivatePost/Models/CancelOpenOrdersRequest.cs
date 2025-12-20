using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CancelOpenOrdersRequest(
    [property: JsonPropertyName("account-id")] string? AccountId = null,
    [property: JsonPropertyName("symbol")] string? Symbol = null,
    [property: JsonPropertyName("side")] string? Side = null,
    [property: JsonPropertyName("size")] string? Size = null,
    [property: JsonPropertyName("price")] string? Price = null,
    [property: JsonPropertyName("created-at")] long? CreatedAt = null);
