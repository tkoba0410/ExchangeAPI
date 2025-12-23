using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record FundingRateResponse(
    [property: JsonPropertyName("current_funding_rate")] decimal CurrentFundingRate,
    [property: JsonPropertyName("next_funding_rate_settledate")] DateTimeOffset NextFundingRateSettleDate);
