using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed record GetBoardStateResponse(
    [property: JsonPropertyName("health")] string? Health,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("data")] string? Data);
