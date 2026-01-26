using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

internal static class BitflyerBoardStateNormalizer
{
    public static BitflyerBoardStateNormalized Normalize(RawPublicModels.BoardStateResponse wire) =>
        new(
            Health: wire.Health,
            State: wire.State,
            Data: wire.Data);
}
