using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BalanceEntryNormalized(
    FreeText Currency,
    FreeText Type,
    decimal Balance);
