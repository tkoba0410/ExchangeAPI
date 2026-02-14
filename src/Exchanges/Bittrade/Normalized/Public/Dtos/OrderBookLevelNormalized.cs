namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record OrderBookLevelNormalized(
    decimal Price,
    decimal Size);
