namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record BitflyerCollateralAccountNormalized(
    string CurrencyCode,
    decimal Amount,
    decimal Available);
