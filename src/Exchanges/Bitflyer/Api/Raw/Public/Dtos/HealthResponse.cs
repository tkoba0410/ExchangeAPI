using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
