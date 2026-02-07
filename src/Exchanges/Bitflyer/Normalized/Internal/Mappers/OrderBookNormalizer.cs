using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class OrderBookNormalizer
{
    public static bool TryNormalize(
        RawPublicDtos.GetBoardResponse wire,
        out OrderBookNormalized? normalized,
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

    private static OrderBookNormalized Build(RawPublicDtos.GetBoardResponse wire) =>
        new(
            Bids: (wire.Bids ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(b => new OrderBookLevelNormalized(b.Price, b.Size))
                .ToArray(),
            Asks: (wire.Asks ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(a => new OrderBookLevelNormalized(a.Price, a.Size))
                .ToArray());
}
