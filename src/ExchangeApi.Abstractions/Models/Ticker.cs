using System;

namespace ExchangeApi.Abstractions.Models
{
    /// <summary>
    /// 現在価格情報（Ticker）。
    /// Stage1 では最小限のフィールドのみを持つ。
    /// </summary>
    public sealed class Ticker
    {
        /// <summary>シンボル（例: "BTC/JPY"）。</summary>
        public string Symbol { get; init; } = string.Empty;

        /// <summary>最良買い気配値。</summary>
        public decimal BestBid { get; init; }

        /// <summary>最良売り気配値。</summary>
        public decimal BestAsk { get; init; }

        /// <summary>最終約定価格。</summary>
        public decimal LastTradedPrice { get; init; }

        /// <summary>価格情報の時刻（UTC）。</summary>
        public DateTime TimestampUtc { get; init; }
    }
}
