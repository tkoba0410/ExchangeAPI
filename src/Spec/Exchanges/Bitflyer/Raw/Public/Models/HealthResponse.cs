using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
