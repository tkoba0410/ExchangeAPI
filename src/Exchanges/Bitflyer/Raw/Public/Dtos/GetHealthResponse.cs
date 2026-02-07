using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

public sealed record GetHealthResponse(
    [property: JsonPropertyName("status")] string? Status);
