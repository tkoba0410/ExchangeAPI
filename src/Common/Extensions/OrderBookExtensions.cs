using System;
using System.Collections.Generic;
using System.Linq;
using Common.Dtos;
namespace Common.Extensions;

/// <summary>OrderBook に対する共通ユーティリティ（両サイドを参照）。</summary>
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

    /// <summary>サイズ指定で買い成行を呑み切る計算（asks 側）。</summary>
    public static FillEstimate CalcBuyPriceBySize(this OrderBook orderBook, decimal takerSize)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        return Fill(orderBook.Asks, takerSize, isBuy: true, targetSize: takerSize, targetPrice: null);
    }

    /// <summary>サイズ指定で売り成行を呑み切る計算（bids 側）。</summary>
    public static FillEstimate CalcSellPriceBySize(this OrderBook orderBook, decimal takerSize)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        return Fill(orderBook.Bids, takerSize, isBuy: false, targetSize: takerSize, targetPrice: null);
    }

    /// <summary>
    /// 価格指定で買い側の約定可能量を計算（asks 側、昇順想定）。
    /// 合計サイズ・合計コスト・平均価格を返す。
    /// </summary>
    public static FillEstimate CalcBuySizeByPrice(this OrderBook orderBook, decimal maxPrice)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        if (maxPrice <= 0) throw new ArgumentOutOfRangeException(nameof(maxPrice));

        decimal totalSize = 0;
        decimal totalValue = 0;
        foreach (var level in orderBook.Asks)
        {
            if (level.Price <= maxPrice)
            {
                totalSize += level.Size;
                totalValue += level.Price * level.Size;
            }
            else
            {
                break; // 昇順を想定
            }
        }
        var avg = totalSize > 0 ? totalValue / totalSize : (decimal?)null;
        var filled = totalSize > 0;
        // 買いなので符号は正
        return new FillEstimate(
            filled,
            SignedSize: totalSize,
            Delta: totalValue,
            EstimatedAveragePrice: avg,
            TargetPrice: maxPrice,
            TargetSize: null);
    }

    /// <summary>
    /// 価格指定で売り側の約定可能量を計算（bids 側、降順想定）。
    /// 合計サイズ・合計受取・平均価格を返す。
    /// </summary>
    public static FillEstimate CalcSellSizeByPrice(this OrderBook orderBook, decimal minPrice)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        if (minPrice <= 0) throw new ArgumentOutOfRangeException(nameof(minPrice));

        decimal totalSize = 0;
        decimal totalValue = 0;
        foreach (var level in orderBook.Bids)
        {
            if (level.Price >= minPrice)
            {
                totalSize += level.Size;
                totalValue += level.Price * level.Size;
            }
            else
            {
                break; // 降順を想定
            }
        }
        var avg = totalSize > 0 ? totalValue / totalSize : (decimal?)null;
        var filled = totalSize > 0;
        // 売りなので符号は負
        return new FillEstimate(
            filled,
            SignedSize: -totalSize,
            Delta: -totalValue,
            EstimatedAveragePrice: avg,
            TargetPrice: minPrice,
            TargetSize: null);
    }

    private static FillEstimate Fill(IReadOnlyList<OrderBookLevel> levels, decimal takerSize, bool isBuy, decimal? targetSize, decimal? targetPrice)
    {
        if (levels is null) throw new ArgumentNullException(nameof(levels));
        if (takerSize <= 0) throw new ArgumentOutOfRangeException(nameof(takerSize));

        decimal remaining = takerSize;
        decimal totalSize = 0;
        decimal totalValue = 0;

        foreach (var level in levels)
        {
            if (remaining <= 0) break;
            var use = Math.Min(level.Size, remaining);
            totalSize += use;
            totalValue += use * level.Price;
            remaining -= use;
        }

        var filled = remaining <= 0;
        var avg = totalSize > 0 ? totalValue / totalSize : (decimal?)null;
        var signedSize = isBuy ? totalSize : -totalSize;
        var delta = isBuy ? totalValue : -totalValue;
        return new FillEstimate(
            filled,
            SignedSize: signedSize,
            Delta: delta,
            EstimatedAveragePrice: avg,
            TargetPrice: targetPrice,
            TargetSize: targetSize);
    }
}
