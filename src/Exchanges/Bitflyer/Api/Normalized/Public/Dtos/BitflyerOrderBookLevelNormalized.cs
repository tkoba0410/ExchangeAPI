namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerOrderBookLevelNormalized(
    decimal Price,
    decimal Size);
