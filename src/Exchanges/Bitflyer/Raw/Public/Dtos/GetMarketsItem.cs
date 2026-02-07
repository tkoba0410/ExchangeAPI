using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

public sealed record GetMarketsItem(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
