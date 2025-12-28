using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Models;

internal sealed record BittradeOrderBookNormalized(
    IReadOnlyList<BittradeOrderBookLevelNormalized> Bids,
    IReadOnlyList<BittradeOrderBookLevelNormalized> Asks);
