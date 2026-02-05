using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerCollateralAccountNormalized(
    CurrencyCode CurrencyCode,
    decimal Amount,
    decimal Available);
