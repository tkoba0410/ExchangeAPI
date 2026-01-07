using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerOrderBookNormalized(
    IReadOnlyList<BitflyerOrderBookLevelNormalized> Bids,
    IReadOnlyList<BitflyerOrderBookLevelNormalized> Asks);
