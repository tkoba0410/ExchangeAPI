using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record GetDepthResponse(
    IReadOnlyList<OrderBookLevelNormalized> Bids,
    IReadOnlyList<OrderBookLevelNormalized> Asks);
