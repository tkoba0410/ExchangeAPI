using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

public sealed record Market(
[property: JsonPropertyName("product_code")] ProductCode ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
