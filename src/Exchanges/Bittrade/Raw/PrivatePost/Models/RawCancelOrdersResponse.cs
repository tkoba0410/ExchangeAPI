using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] object? Data);
