using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerBoardStateNormalized(
    FreeText? Health,
    FreeText? State,
    FreeText? Data);
