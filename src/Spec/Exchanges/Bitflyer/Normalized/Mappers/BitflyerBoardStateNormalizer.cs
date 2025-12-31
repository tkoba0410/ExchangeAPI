using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerBoardStateNormalizer
{
    public static BitflyerBoardStateNormalized Normalize(BoardStateResponse wire) =>
        new(
            Health: wire.Health,
            State: wire.State,
            Data: wire.Data);
}
