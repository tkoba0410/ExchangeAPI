namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerCorporateLeverageNormalized(
    decimal CurrentMax,
    DateTimeOffset CurrentStartDate,
    decimal? NextMax,
    DateTimeOffset? NextStartDate);
