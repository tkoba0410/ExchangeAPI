using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Contract.Dtos;

/// <summary>
/// 板差分を適用するための内部バッファ。内部では SortedDictionary を保持し、スナップショットは OrderBook に変換して返す。
/// </summary>
public sealed class OrderBookBuffer
{
    private readonly SortedDictionary<decimal, decimal> _bids;
    private readonly SortedDictionary<decimal, decimal> _asks;

    public OrderBookBuffer()
    {
        _bids = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Create((x, y) => y.CompareTo(x))); // 降順
        _asks = new SortedDictionary<decimal, decimal>(Comparer<decimal>.Default); // 昇順
    }

    public OrderBookBuffer(IEnumerable<OrderBookLevel> bids, IEnumerable<OrderBookLevel> asks)
        : this()
    {
        ApplyLevels(bids, asks);
    }

    public void Clear()
    {
        _bids.Clear();
        _asks.Clear();
    }

    public void ApplySnapshot(OrderBook snapshot)
    {
        if (snapshot is null) throw new ArgumentNullException(nameof(snapshot));
        Clear();
        ApplyLevels(snapshot.Bids, snapshot.Asks);
    }

    public void ApplyLevels(IEnumerable<OrderBookLevel> bids, IEnumerable<OrderBookLevel> asks)
    {
        if (bids is null) throw new ArgumentNullException(nameof(bids));
        if (asks is null) throw new ArgumentNullException(nameof(asks));

        foreach (var level in bids)
        {
            AddOrUpdateBid(level.Price, level.Size);
        }

        foreach (var level in asks)
        {
            AddOrUpdateAsk(level.Price, level.Size);
        }
    }

    public void AddOrUpdateBid(decimal price, decimal size)
    {
        if (size <= 0)
        {
            _bids.Remove(price);
            return;
        }
        _bids[price] = size;
    }

    public void AddOrUpdateAsk(decimal price, decimal size)
    {
        if (size <= 0)
        {
            _asks.Remove(price);
            return;
        }
        _asks[price] = size;
    }

    public OrderBook ToOrderBook()
    {
        var bids = _bids.Select(kv => new OrderBookLevel(kv.Key, kv.Value)).ToList();
        var asks = _asks.Select(kv => new OrderBookLevel(kv.Key, kv.Value)).ToList();
        return new OrderBook(bids, asks);
    }
}
