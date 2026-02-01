namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerFundingRateNormalized(
    decimal CurrentFundingRate,
    DateTimeOffset NextFundingRateSettleDate);
