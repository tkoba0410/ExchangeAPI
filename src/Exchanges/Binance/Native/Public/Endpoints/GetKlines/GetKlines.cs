using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;

public static class GetKlines
{
    public sealed class Item
    {
        [JsonPropertyName("open_time")]
        public required long OpenTime { get; init; }
        [JsonPropertyName("open_price")]
        public required decimal OpenPrice { get; init; }
        [JsonPropertyName("high_price")]
        public required decimal HighPrice { get; init; }
        [JsonPropertyName("low_price")]
        public required decimal LowPrice { get; init; }
        [JsonPropertyName("close_price")]
        public required decimal ClosePrice { get; init; }
        [JsonPropertyName("volume")]
        public required decimal Volume { get; init; }
        [JsonPropertyName("close_time")]
        public required long CloseTime { get; init; }
        [JsonPropertyName("quote_asset_volume")]
        public required decimal QuoteAssetVolume { get; init; }
        [JsonPropertyName("number_of_trades")]
        public required int NumberOfTrades { get; init; }
        [JsonPropertyName("taker_buy_base_asset_volume")]
        public required decimal TakerBuyBaseAssetVolume { get; init; }
        [JsonPropertyName("taker_buy_quote_asset_volume")]
        public required decimal TakerBuyQuoteAssetVolume { get; init; }
    }
}
