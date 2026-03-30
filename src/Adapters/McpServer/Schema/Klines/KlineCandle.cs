using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Klines;

public sealed class KlineCandle
{
    [JsonPropertyName("openTime")]
    public required string OpenTime { get; init; }

    [JsonPropertyName("closeTime")]
    public required string CloseTime { get; init; }

    [JsonPropertyName("open")]
    public required string Open { get; init; }

    [JsonPropertyName("high")]
    public required string High { get; init; }

    [JsonPropertyName("low")]
    public required string Low { get; init; }

    [JsonPropertyName("close")]
    public required string Close { get; init; }

    [JsonPropertyName("volume")]
    public required string Volume { get; init; }

    [JsonPropertyName("quoteVolume")]
    public required string QuoteVolume { get; init; }

    [JsonPropertyName("tradeCount")]
    public required int TradeCount { get; init; }

    [JsonPropertyName("takerBuyBaseVolume")]
    public required string TakerBuyBaseVolume { get; init; }

    [JsonPropertyName("takerBuyQuoteVolume")]
    public required string TakerBuyQuoteVolume { get; init; }
}
