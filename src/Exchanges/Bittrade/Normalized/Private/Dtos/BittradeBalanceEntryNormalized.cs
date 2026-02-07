using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeBalanceEntryNormalized(
    FreeText Currency,
    FreeText Type,
    decimal Balance);
