using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed record Market(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
