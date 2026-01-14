using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

public sealed record Market(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
