namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

public sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
