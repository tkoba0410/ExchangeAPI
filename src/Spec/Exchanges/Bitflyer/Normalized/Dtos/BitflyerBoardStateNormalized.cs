namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerBoardStateNormalized(
    string? Health,
    string? State,
    string? Data);
