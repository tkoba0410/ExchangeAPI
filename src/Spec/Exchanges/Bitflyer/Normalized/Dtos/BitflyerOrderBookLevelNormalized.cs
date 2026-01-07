namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
