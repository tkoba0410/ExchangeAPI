using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeOrderBookNormalized(
    IReadOnlyList<BittradeOrderBookLevelNormalized> Bids,
    IReadOnlyList<BittradeOrderBookLevelNormalized> Asks);
