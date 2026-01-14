using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;

public sealed record BittradeOrderBookNormalized(
    IReadOnlyList<BittradeOrderBookLevelNormalized> Bids,
    IReadOnlyList<BittradeOrderBookLevelNormalized> Asks);
