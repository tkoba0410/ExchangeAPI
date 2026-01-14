using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

public sealed record CorporateLeverageResponse(
    [property: JsonPropertyName("current_max")] decimal CurrentMax,
    [property: JsonPropertyName("current_startdate")] DateTimeOffset CurrentStartDate,
    [property: JsonPropertyName("next_max")] decimal? NextMax,
    [property: JsonPropertyName("next_startdate")] DateTimeOffset? NextStartDate);
