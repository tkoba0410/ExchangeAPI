using System;
using System.Collections.Generic;
using System.Linq;
using Common.Contract.Dtos;

namespace Common.Contract.Extensions;

/// <summary>OrderBook に対する共通ユーティリティ。</summary>
public static class OrderBookExtensions
{
    /// <summary>最良買い気配（Bid）。板が空なら null。</summary>
    public static decimal? GetBestBid(this OrderBook orderBook)
    {
        var first = orderBook?.Bids?.FirstOrDefault();
        return first?.Price;
    }

    /// <summary>最良売り気配（Ask）。板が空なら null。</summary>
    public static decimal? GetBestAsk(this OrderBook orderBook)
    {
        var first = orderBook?.Asks?.FirstOrDefault();
        return first?.Price;
    }

    /// <summary>ミッドプライス（(Bid+Ask)/2）。どちらか欠損なら null。</summary>
    public static decimal? GetMidPrice(this OrderBook orderBook)
    {
        var bid = orderBook.GetBestBid();
        var ask = orderBook.GetBestAsk();
        if (bid is null || ask is null) return null;
        return (bid.Value + ask.Value) / 2m;
    }

    /// <summary>スプレッド（Ask-Bid）。どちらか欠損なら null。</summary>
    public static decimal? GetSpread(this OrderBook orderBook)
    {
        var bid = orderBook.GetBestBid();
        var ask = orderBook.GetBestAsk();
        if (bid is null || ask is null) return null;
        return ask.Value - bid.Value;
    }

    /// <summary>総サイズ（全レベルの size 合計）。</summary>
    public static decimal GetTotalSize(this OrderBook orderBook) =>
        (orderBook?.Asks?.Sum(x => x.Size) ?? 0m) + (orderBook?.Bids?.Sum(x => x.Size) ?? 0m);

    /// <summary>加重平均価格（価格×数量の総和 / サイズ総和）。対象が空なら null。</summary>
    public static decimal? GetAveragePrice(this IEnumerable<OrderBookLevel> levels)
    {
        if (levels is null) return null;
        decimal totalSize = 0;
        decimal totalValue = 0;
        foreach (var level in levels)
        {
            var price = level.Price;
            var size = level.Size;
            totalSize += size;
            totalValue += price * size;
        }
        return totalSize == 0 ? null : totalValue / totalSize;
    }

    /// <summary>指定サイズを充足するまでのレベルを返す（Filled=false なら未充足）。</summary>
    public static (IReadOnlyList<OrderBookLevel> Levels, bool Filled)
        TakeBySize(this IEnumerable<OrderBookLevel> levels, decimal takerSize)
    {
        if (levels is null) throw new ArgumentNullException(nameof(levels));
        if (takerSize <= 0) throw new ArgumentOutOfRangeException(nameof(takerSize));

        var result = new List<OrderBookLevel>();
        var remaining = takerSize;
        foreach (var level in levels)
        {
            if (remaining <= 0) break;
            var used = Math.Min(level.Size, remaining);
            result.Add(new OrderBookLevel(level.Price, used));
            remaining -= used;
        }

        return (result, remaining <= 0);
    }

    /// <summary>
    /// 価格レンジで切り出す。startPrice から range 分（正なら上方向、負なら下方向）を返す。
    /// includeStart が true の場合、startPrice と同値も含める。
    /// </summary>
    public static IReadOnlyList<OrderBookLevel> TakeByPrice(
        this IEnumerable<OrderBookLevel> levels,
        decimal startPrice,
        decimal range,
        bool includeStart = true)
    {
        if (levels is null) throw new ArgumentNullException(nameof(levels));
        if (range == 0) return Array.Empty<OrderBookLevel>();

        var upperSide = range > 0;
        var bound = startPrice + range;
        var list = new List<OrderBookLevel>();

        foreach (var level in levels)
        {
            var price = level.Price;
            var beyondStart = includeStart ? price >= startPrice : price > startPrice;
            if (!beyondStart) continue;

            if (upperSide && price > bound) break;
            if (!upperSide && price < bound) break;

            list.Add(level);
        }

        return list;
    }

    /// <summary>累積深さ (price, cumulativeSize) を返す。</summary>
    public static IReadOnlyList<(decimal price, decimal cumulative)> Depth(
        this IEnumerable<OrderBookLevel> levels)
    {
        if (levels is null) throw new ArgumentNullException(nameof(levels));

        var list = new List<(decimal price, decimal cumulative)>();
        decimal cum = 0;
        foreach (var level in levels)
        {
            cum += level.Size;
            list.Add((level.Price, cum));
        }
        return list;
    }
}
