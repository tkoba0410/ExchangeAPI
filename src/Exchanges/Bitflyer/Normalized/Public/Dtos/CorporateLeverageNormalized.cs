namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record CorporateLeverageNormalized(
    decimal CurrentMax,
    DateTimeOffset CurrentStartDate,
    decimal? NextMax,
    DateTimeOffset? NextStartDate);
