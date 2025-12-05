using System;
using System.Collections.Generic;

namespace ExchangeApi.Abstractions.Dtos;

public sealed record TickerTick(decimal BestBid, decimal BestAsk, decimal? LastTradedPrice, DateTimeOffset Timestamp);

public sealed record OrderBookDelta(
    IReadOnlyList<OrderBookLevel> Bids,
    IReadOnlyList<OrderBookLevel> Asks,
    bool Snapshot,
    DateTimeOffset Timestamp);

public sealed record ExecutionTick(OrderSide Side, decimal Price, decimal Size, DateTimeOffset Timestamp);
