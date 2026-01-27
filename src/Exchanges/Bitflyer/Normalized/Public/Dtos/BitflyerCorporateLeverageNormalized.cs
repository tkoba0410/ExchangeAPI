namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerCorporateLeverageNormalized(
    decimal CurrentMax,
    DateTimeOffset CurrentStartDate,
    decimal? NextMax,
    DateTimeOffset? NextStartDate);
