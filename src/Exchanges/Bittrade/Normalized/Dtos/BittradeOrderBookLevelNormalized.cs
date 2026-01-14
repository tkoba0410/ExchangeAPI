namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;

public sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
