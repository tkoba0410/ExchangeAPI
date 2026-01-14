using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
