using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record BitflyerMarket(
[property: JsonPropertyName("product_code")] ProductCode ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
