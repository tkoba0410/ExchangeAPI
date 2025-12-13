using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed record BitflyerBoardStateResponse(
    [property: JsonPropertyName("health")] string? Health,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("data")] string? Data);
