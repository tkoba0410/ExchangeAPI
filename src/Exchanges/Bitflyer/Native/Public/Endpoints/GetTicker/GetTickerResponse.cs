using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;

public sealed class GetTickerResponse
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("timestamp")]
    public required DateTimeOffset Timestamp { get; init; }

    [JsonPropertyName("tick_id")]
    public required long TickId { get; init; }

    [JsonPropertyName("best_bid")]
    public required decimal BestBid { get; init; }

    [JsonPropertyName("best_ask")]
    public required decimal BestAsk { get; init; }

    [JsonPropertyName("best_bid_size")]
    public required decimal BestBidSize { get; init; }

    [JsonPropertyName("best_ask_size")]
    public required decimal BestAskSize { get; init; }

    [JsonPropertyName("total_bid_depth")]
    public required decimal TotalBidDepth { get; init; }

    [JsonPropertyName("total_ask_depth")]
    public required decimal TotalAskDepth { get; init; }

    [JsonPropertyName("market_bid_size")]
    public required decimal MarketBidSize { get; init; }

    [JsonPropertyName("market_ask_size")]
    public required decimal MarketAskSize { get; init; }

    [JsonPropertyName("ltp")]
    public required decimal Ltp { get; init; }

    [JsonPropertyName("volume")]
    public required decimal Volume { get; init; }

    [JsonPropertyName("volume_by_product")]
    public required decimal VolumeByProduct { get; init; }
}
