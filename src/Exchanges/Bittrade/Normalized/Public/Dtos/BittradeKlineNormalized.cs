namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record BittradeKlineNormalized(
    string Id,
    decimal Open,
    decimal Close,
    decimal Low,
    decimal High,
    decimal Amount,
    decimal Volume,
    long Count);
