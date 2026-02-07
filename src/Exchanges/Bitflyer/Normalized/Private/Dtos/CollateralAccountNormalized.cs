using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record CollateralAccountNormalized(
    CurrencyCode CurrencyCode,
    decimal Amount,
    decimal Available);
