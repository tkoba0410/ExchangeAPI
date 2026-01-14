namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;

public sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
