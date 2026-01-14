namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
