using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

public sealed record GetFundingRateResponse(
    [property: JsonPropertyName("current_funding_rate")] decimal CurrentFundingRate,
    [property: JsonPropertyName("next_funding_rate_settledate")] DateTimeOffset NextFundingRateSettleDate);
