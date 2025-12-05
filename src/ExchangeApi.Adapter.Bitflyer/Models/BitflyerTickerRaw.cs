using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models
{
    /// <summary>
    /// bitFlyer Public REST API の <c>GET /v1/getticker</c> レスポンスを
    /// ほぼそのまま写像した Raw モデル。
    /// 
    /// - ExchangeApi.Core の Ticker とは 1:1 ではなく、
    ///   取引所固有のフィールドもすべて保持するための内部用 DTO
    /// - Stage1 では Bitflyer アダプタ内でのみ使用し、外部には公開しない
    /// </summary>
    public sealed class BitflyerTickerRaw
    {
        /// <summary>
        /// プロダクトコード。
        /// 例: <c>BTC_JPY</c>。
        /// </summary>
        [JsonPropertyName("product_code")]
        public string ProductCode { get; init; } = string.Empty;

        /// <summary>
        /// ティッカーの発生時刻。
        /// bitFlyer のレスポンスでは ISO8601 文字列だが、
        /// ここでは DateTimeOffset としてパース済みで保持する。
        /// 通常は UTC (Z) を表す。
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTimeOffset Timestamp { get; init; }

        [JsonPropertyName("tick_id")]
        public long TickId { get; init; }
        /// <summary>
        /// 最良買い気配価格。
        /// JSON フィールド <c>best_bid</c> に対応。
        /// </summary>
        [JsonPropertyName("best_bid")]
        public decimal BestBid { get; init; }
        /// <summary>
        /// 最良売り気配価格。
        /// JSON フィールド <c>best_ask</c> に対応。
        /// </summary>
        [JsonPropertyName("best_ask")]
        public decimal BestAsk { get; init; }

        [JsonPropertyName("best_bid_size")]
        public decimal BestBidSize { get; init; }

        [JsonPropertyName("best_ask_size")]
        public decimal BestAskSize { get; init; }

        [JsonPropertyName("total_bid_depth")]
        public decimal TotalBidDepth { get; init; }

        [JsonPropertyName("total_ask_depth")]
        public decimal TotalAskDepth { get; init; }

        /// <summary>
        /// 直近約定価格 (Last Traded Price)。
        /// JSON フィールド <c>ltp</c> に対応。
        /// </summary>
        [JsonPropertyName("ltp")]
        public decimal LastTradedPrice { get; init; }
        /// <summary>
        /// 24 時間出来高。
        /// JSON フィールド <c>volume</c> に対応。
        /// </summary>
        [JsonPropertyName("volume")]
        public decimal Volume { get; init; }
        /// <summary>
        /// プロダクトごとの 24 時間出来高。
        /// JSON フィールド <c>volume_by_product</c> に対応。
        /// </summary>
        [JsonPropertyName("volume_by_product")]
        public decimal VolumeByProduct { get; init; }
    }
}
