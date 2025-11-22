using System.Text.Json.Serialization;

namespace ExchangeApi.Bitflyer.Models
{
    /// <summary>
    /// bitFlyer Public REST GET /v1/getticker の Raw レスポンス。
    /// 取引所仕様を欠損なく保持するためのモデル。
    /// </summary>
    public sealed class BitflyerTickerRaw
    {
        [JsonPropertyName("product_code")]
        public string ProductCode { get; init; } = string.Empty;

        // 公式レスポンスでは ISO8601 文字列のため string で保持する
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; init; } = string.Empty;

        [JsonPropertyName("tick_id")]
        public long TickId { get; init; }

        [JsonPropertyName("best_bid")]
        public decimal BestBid { get; init; }

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

        // last traded price
        [JsonPropertyName("ltp")]
        public decimal LastTradedPrice { get; init; }

        [JsonPropertyName("volume")]
        public decimal Volume { get; init; }

        [JsonPropertyName("volume_by_product")]
        public decimal VolumeByProduct { get; init; }
    }
}
