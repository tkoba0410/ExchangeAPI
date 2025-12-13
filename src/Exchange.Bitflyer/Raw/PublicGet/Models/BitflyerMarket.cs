using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed record BitflyerMarket(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
