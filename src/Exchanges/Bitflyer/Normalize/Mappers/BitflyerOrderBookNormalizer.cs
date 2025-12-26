using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;

internal static class BitflyerOrderBookNormalizer
{
    public static BitflyerOrderBookNormalized Normalize(Board wire) =>
        new(
            Bids: (wire.Bids ?? Array.Empty<BoardEntry>())
                .Select(b => new BitflyerOrderBookLevelNormalized(b.Price, b.Size))
                .ToArray(),
            Asks: (wire.Asks ?? Array.Empty<BoardEntry>())
                .Select(a => new BitflyerOrderBookLevelNormalized(a.Price, a.Size))
                .ToArray());
}
