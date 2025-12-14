using System;
using System.Collections.Generic;
using System.Linq;
using Common.Contract.Dtos;

namespace Common.Contract.Extensions;

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

    /// <summary>
    /// 指定価格以下で約定可能な買い側の集計（asks 側、昇順想定）。
    /// 合計サイズ・合計コスト・平均価格を返す。
    /// </summary>
    public static MarketFillResult CalcExecutableSizeForBuy(this OrderBook orderBook, decimal maxPrice)
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
        return new MarketFillResult(filled, totalSize, totalValue, avg);
    }

    /// <summary>
    /// 指定価格以上で約定可能な売り側の集計（bids 側、降順想定）。
    /// 合計サイズ・合計受取・平均価格を返す。
    /// </summary>
    public static MarketFillResult CalcExecutableSizeForSell(this OrderBook orderBook, decimal minPrice)
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
        return new MarketFillResult(filled, totalSize, totalValue, avg);
    }

    /// <summary>成行買いで指定サイズを呑み切る計算（asks 側を上から食い進める）。</summary>
    public static MarketFillResult CalcMarketBuy(this OrderBook orderBook, decimal takerSize)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        return Fill(orderBook.Asks, takerSize);
    }

    /// <summary>成行売りで指定サイズを呑み切る計算（bids 側を下から食い進める）。</summary>
    public static MarketFillResult CalcMarketSell(this OrderBook orderBook, decimal takerSize)
    {
        if (orderBook is null) throw new ArgumentNullException(nameof(orderBook));
        return Fill(orderBook.Bids, takerSize);
    }

    private static MarketFillResult Fill(IReadOnlyList<OrderBookLevel> levels, decimal takerSize)
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
        return new MarketFillResult(filled, totalSize, totalValue, avg);
    }
}
