using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire;

public sealed record BoardStateResponse(
    [property: JsonPropertyName("health")] string? Health,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("data")] string? Data);
