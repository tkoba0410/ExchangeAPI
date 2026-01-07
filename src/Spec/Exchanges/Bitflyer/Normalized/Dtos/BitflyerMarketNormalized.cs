namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerMarketNormalized(
    string ProductCode,
    string? Alias);
