using System.Collections.Generic;

namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 板スナップショット。
/// </summary>
public sealed record OrderBook(
    IReadOnlyList<OrderBookLevel> Bids,
    IReadOnlyList<OrderBookLevel> Asks,
    decimal? MidPrice = null);

/// <summary>
/// 板の価格レベル。
/// </summary>
public sealed record OrderBookLevel(decimal Price, decimal Size);
