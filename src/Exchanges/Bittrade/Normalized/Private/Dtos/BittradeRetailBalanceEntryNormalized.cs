namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeRetailBalanceEntryNormalized(
    string Currency,
    decimal? Balance,
    decimal? Available,
    decimal? Frozen);
