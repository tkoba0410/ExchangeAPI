namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;

public sealed record BitflyerCollateralAccountNormalized(
    string CurrencyCode,
    decimal Amount,
    decimal Available);
