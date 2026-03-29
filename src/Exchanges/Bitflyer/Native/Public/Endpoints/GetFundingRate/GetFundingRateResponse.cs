using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;

public sealed class GetFundingRateResponse
{
    [JsonPropertyName("current_funding_rate")]
    public required decimal CurrentFundingRate { get; init; }
    [JsonPropertyName("next_funding_rate_settle_date")]
    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset NextFundingRateSettleDate { get; init; }
}
