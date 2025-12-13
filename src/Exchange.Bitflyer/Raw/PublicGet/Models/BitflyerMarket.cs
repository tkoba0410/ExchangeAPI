using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PublicGet.Models;

public sealed record BitflyerMarket(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
