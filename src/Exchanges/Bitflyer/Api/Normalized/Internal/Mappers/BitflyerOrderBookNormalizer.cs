using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerOrderBookNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.GetBoardResponse wire,
        out BitflyerOrderBookNormalized? normalized,
        out CallError? error)
    {
        try
        {
            normalized = Build(wire);
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            normalized = null;
            error = new CallError(CallErrorKind.Mapping, "bitFlyer order book response invalid.", ex);
            return false;
        }
    }

    private static BitflyerOrderBookNormalized Build(RawPublicDtos.GetBoardResponse wire) =>
        new(
            Bids: (wire.Bids ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(b => new BitflyerOrderBookLevelNormalized(b.Price, b.Size))
                .ToArray(),
            Asks: (wire.Asks ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(a => new BitflyerOrderBookLevelNormalized(a.Price, a.Size))
                .ToArray());
}
