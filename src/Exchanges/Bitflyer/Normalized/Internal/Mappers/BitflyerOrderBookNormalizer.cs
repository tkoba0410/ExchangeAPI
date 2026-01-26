using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;
using RawPublicModels = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;

internal static class BitflyerOrderBookNormalizer
{
    public static BitflyerOrderBookNormalized Normalize(RawPublicModels.Board wire) =>
        new(
            Bids: (wire.Bids ?? Array.Empty<RawPublicModels.BoardEntry>())
                .Select(b => new BitflyerOrderBookLevelNormalized(b.Price, b.Size))
                .ToArray(),
            Asks: (wire.Asks ?? Array.Empty<RawPublicModels.BoardEntry>())
                .Select(a => new BitflyerOrderBookLevelNormalized(a.Price, a.Size))
                .ToArray());
}
