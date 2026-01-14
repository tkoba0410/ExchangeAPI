namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerMarketNormalized(
    string ProductCode,
    string? Alias);
