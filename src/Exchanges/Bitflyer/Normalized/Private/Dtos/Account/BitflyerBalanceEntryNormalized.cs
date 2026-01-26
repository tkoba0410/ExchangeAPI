using ExchangeApi.Primitives.DomainCommon.Enums;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;

public sealed record BitflyerBalanceEntryNormalized(
    string Currency,
    decimal Amount,
    decimal Available,
    CurrencyCode CurrencyCode = CurrencyCode.Unknown);
