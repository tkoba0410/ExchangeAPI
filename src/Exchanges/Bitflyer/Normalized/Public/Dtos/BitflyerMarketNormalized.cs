namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerMarketNormalized(
    string ProductCode,
    string? Alias);
