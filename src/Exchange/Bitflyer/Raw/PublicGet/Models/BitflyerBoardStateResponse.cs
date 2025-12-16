using System.Text.Json.Serialization;
namespace Exchange.Bitflyer.Raw;

public sealed record BitflyerBoardStateResponse(
    [property: JsonPropertyName("health")] string? Health,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("data")] string? Data);
