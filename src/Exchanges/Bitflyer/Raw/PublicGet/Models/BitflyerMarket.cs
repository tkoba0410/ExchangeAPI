using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;

public sealed record Market(
[property: JsonPropertyName("product_code")] ProductCode ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
