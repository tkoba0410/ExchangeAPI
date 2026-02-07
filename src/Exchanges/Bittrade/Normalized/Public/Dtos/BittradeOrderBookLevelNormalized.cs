namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
