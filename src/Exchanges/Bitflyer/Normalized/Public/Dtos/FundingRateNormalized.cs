namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record FundingRateNormalized(
    decimal CurrentFundingRate,
    DateTimeOffset NextFundingRateSettleDate);
