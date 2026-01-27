using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Public.Dtos;

public sealed class Ticker
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = string.Empty;

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; init; }

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

    [JsonPropertyName("ltp")]
    public decimal LastTradedPrice { get; init; }

    [JsonPropertyName("volume")]
    public decimal Volume { get; init; }

    [JsonPropertyName("volume_by_product")]
    public decimal VolumeByProduct { get; init; }
}
