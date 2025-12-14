using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Contract.Dtos;

/// <summary>
/// 板スナップショット。Bids は価格降順、Asks は価格昇順で内部整列される。
/// </summary>
public sealed record OrderBook
{
    public IReadOnlyList<OrderBookLevel> Bids { get; init; }
    public IReadOnlyList<OrderBookLevel> Asks { get; init; }
    public decimal? MidPrice { get; init; }

    public OrderBook(IEnumerable<OrderBookLevel> bids, IEnumerable<OrderBookLevel> asks, decimal? midPrice = null)
    {
        if (bids is null) throw new ArgumentNullException(nameof(bids));
        if (asks is null) throw new ArgumentNullException(nameof(asks));

        Bids = SortDescending(bids);
        Asks = SortAscending(asks);
        MidPrice = midPrice;
    }

    private static IReadOnlyList<OrderBookLevel> SortDescending(IEnumerable<OrderBookLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x)));
        foreach (var level in levels)
        {
            dict[level.Price] = dict.TryGetValue(level.Price, out var size)
                ? size + level.Size
                : level.Size;
        }
        return dict.Select(kv => new OrderBookLevel(kv.Key, kv.Value)).ToList();
    }

    private static IReadOnlyList<OrderBookLevel> SortAscending(IEnumerable<OrderBookLevel> levels)
    {
        var dict = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Default);
        foreach (var level in levels)
        {
            dict[level.Price] = dict.TryGetValue(level.Price, out var size)
                ? size + level.Size
                : level.Size;
        }
        return dict.Select(kv => new OrderBookLevel(kv.Key, kv.Value)).ToList();
    }
}

/// <summary>
/// 板の価格レベル。
/// </summary>
public sealed record OrderBookLevel(decimal Price, decimal Size);
