using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerOrderBookNormalized(
    IReadOnlyList<BitflyerOrderBookLevelNormalized> Bids,
    IReadOnlyList<BitflyerOrderBookLevelNormalized> Asks);
