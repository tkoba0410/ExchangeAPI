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
}
