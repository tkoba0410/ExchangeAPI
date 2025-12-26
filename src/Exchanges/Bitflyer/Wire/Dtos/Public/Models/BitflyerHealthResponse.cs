using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
