using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;
using RawPublicDtos = ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Dtos;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Mappers;

internal static class BitflyerOrderBookNormalizer
{
    public static BitflyerOrderBookNormalized Normalize(RawPublicDtos.Board wire) =>
        new(
            Bids: (wire.Bids ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(b => new BitflyerOrderBookLevelNormalized(b.Price, b.Size))
                .ToArray(),
            Asks: (wire.Asks ?? Array.Empty<RawPublicDtos.BoardEntry>())
                .Select(a => new BitflyerOrderBookLevelNormalized(a.Price, a.Size))
                .ToArray());
}
