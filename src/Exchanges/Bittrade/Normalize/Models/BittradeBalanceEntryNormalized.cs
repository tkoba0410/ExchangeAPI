namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

internal sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
