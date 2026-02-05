using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerBalanceEntryNormalized(
    CurrencyCode CurrencyCode,
    decimal Amount,
    decimal Available);
