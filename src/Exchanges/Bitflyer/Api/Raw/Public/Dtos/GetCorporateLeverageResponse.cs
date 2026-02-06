using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed record GetCorporateLeverageResponse(
    [property: JsonPropertyName("current_max")] decimal CurrentMax,
    [property: JsonPropertyName("current_startdate")] DateTimeOffset CurrentStartDate,
    [property: JsonPropertyName("next_max")] decimal? NextMax,
    [property: JsonPropertyName("next_startdate")] DateTimeOffset? NextStartDate);
