using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeRetailBalanceEntryNormalized(
    FreeText Currency,
    decimal? Balance,
    decimal? Available,
    decimal? Frozen);
