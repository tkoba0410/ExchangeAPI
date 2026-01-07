namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
