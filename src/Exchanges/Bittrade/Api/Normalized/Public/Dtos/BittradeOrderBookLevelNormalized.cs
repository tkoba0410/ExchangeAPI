namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;

public sealed record BittradeOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
