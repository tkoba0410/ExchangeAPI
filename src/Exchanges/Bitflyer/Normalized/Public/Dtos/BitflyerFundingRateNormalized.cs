namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerFundingRateNormalized(
    decimal CurrentFundingRate,
    DateTimeOffset NextFundingRateSettleDate);
