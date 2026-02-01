using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerBoardStateNormalizer
{
    public static BitflyerBoardStateNormalized Normalize(RawPublicDtos.BoardStateResponse wire) =>
        new(
            Health: wire.Health,
            State: wire.State,
            Data: wire.Data);
}
