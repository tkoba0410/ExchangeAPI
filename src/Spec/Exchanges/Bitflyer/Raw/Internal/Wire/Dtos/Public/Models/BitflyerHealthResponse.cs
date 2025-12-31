using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

internal sealed record HealthResponse(
    [property: JsonPropertyName("status")] string? Status);
