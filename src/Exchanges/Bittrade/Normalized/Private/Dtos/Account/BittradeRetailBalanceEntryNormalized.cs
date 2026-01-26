namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;

public sealed record BittradeRetailBalanceEntryNormalized(
    string Currency,
    decimal? Balance,
    decimal? Available,
    decimal? Frozen);
