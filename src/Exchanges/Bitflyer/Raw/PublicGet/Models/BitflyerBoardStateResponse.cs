using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record BitflyerBoardStateResponse(
    [property: JsonPropertyName("health")] string? Health,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("data")] string? Data);
