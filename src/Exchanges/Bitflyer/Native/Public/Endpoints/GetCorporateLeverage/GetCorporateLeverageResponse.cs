using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;

public sealed class GetCorporateLeverageResponse
{
    [JsonPropertyName("current_max")]
    public required decimal CurrentMax { get; init; }
    [JsonPropertyName("current_start_date")]
    public required DateTimeOffset CurrentStartDate { get; init; }
    [JsonPropertyName("next_max")]
    public decimal? NextMax { get; init; }
    [JsonPropertyName("next_start_date")]
    public DateTimeOffset? NextStartDate { get; init; }
}
