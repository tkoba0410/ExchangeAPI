using System;
using System.Collections.Generic;
using Common.Contract.Dtos;

namespace Common.Contract.Extensions;

/// <summary>片側（IEnumerable&lt;OrderBookLevel&gt;）に対するユーティリティ。</summary>
public static class OrderBookLevelExtensions
{
    /// <summary>サイズ合計。</summary>
    public static decimal SumSize(this IEnumerable<OrderBookLevel> levels)
    {
        if (levels is null) throw new ArgumentNullException(nameof(levels));
        decimal total = 0;
        foreach (var level in levels)
        {
            total += level.Size;
        }
        return total;
    }

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
