using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public;

public sealed record Market(
    [property: JsonPropertyName("product_code")] RawProductCode ProductCode,
    [property: JsonPropertyName("alias")] string? Alias);
