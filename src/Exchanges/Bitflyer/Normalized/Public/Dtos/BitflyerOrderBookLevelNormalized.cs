namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
