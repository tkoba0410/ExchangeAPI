using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record OrderBookNormalized(
    decimal MidPrice,
    IReadOnlyList<OrderBookLevelNormalized> Bids,
    IReadOnlyList<OrderBookLevelNormalized> Asks);
