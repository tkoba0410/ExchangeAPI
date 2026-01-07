namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
