using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>
/// 板スナップショット。
/// </summary>
public sealed record OrderBook(
    IReadOnlyList<OrderBookLevel> Bids,
    IReadOnlyList<OrderBookLevel> Asks);

/// <summary>
/// 板の価格レベル。
/// </summary>
public sealed record OrderBookLevel(Price Price, Size Size);
