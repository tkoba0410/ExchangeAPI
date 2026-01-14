namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerBoardStateNormalized(
    string? Health,
    string? State,
    string? Data);
