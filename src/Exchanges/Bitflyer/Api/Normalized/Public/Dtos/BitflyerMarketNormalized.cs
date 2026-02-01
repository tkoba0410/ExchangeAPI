namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerMarketNormalized(
    string ProductCode,
    string? Alias);
