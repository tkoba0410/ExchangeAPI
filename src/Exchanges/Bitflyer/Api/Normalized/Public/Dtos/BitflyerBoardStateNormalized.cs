namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerBoardStateNormalized(
    string? Health,
    string? State,
    string? Data);
