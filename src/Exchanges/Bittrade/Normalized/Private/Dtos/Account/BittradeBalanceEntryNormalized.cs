namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Account;

public sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
