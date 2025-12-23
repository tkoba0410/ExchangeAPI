using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;

public sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
