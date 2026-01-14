using System;
using System.Linq;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;

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
