using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerBalanceEntryNormalized(
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown);
