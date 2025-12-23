using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
