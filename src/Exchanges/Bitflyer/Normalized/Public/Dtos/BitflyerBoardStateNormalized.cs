namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerBoardStateNormalized(
    string? Health,
    string? State,
    string? Data);
