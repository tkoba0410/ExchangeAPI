namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeBalanceEntryNormalized(
    string Currency,
    string Type,
    decimal Balance);
