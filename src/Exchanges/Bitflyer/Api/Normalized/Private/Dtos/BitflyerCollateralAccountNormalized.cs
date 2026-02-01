namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerCollateralAccountNormalized(
    string CurrencyCode,
    decimal Amount,
    decimal Available);
