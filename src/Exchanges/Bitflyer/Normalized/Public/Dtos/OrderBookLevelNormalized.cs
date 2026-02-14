namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record OrderBookLevelNormalized(
    decimal Price,
    decimal Size);
