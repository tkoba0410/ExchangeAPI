using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Common.Dtos;

/// <summary>
/// 板スナップショット。Bids は価格降順、Asks は価格昇順で内部整列される。
/// </summary>
public sealed record OrderBook
{
    public ExchangeCode ExchangeCode { get; init; }
    public IReadOnlyList<OrderBookLevel> Bids { get; init; }
    public IReadOnlyList<OrderBookLevel> Asks { get; init; }

    public OrderBook(ExchangeCode exchangeCode, IEnumerable<OrderBookLevel> bids, IEnumerable<OrderBookLevel> asks)
    {
        ExchangeCode = exchangeCode;
        if (bids is null) throw new ArgumentNullException(nameof(bids));
        if (asks is null) throw new ArgumentNullException(nameof(asks));

        Bids = SortDescending(bids);
        Asks = SortAscending(asks);
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
