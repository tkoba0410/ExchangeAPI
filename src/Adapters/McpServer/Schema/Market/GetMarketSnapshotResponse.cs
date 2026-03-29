using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Market;

public sealed class GetMarketSnapshotResponse
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("bid")]
    public required string Bid { get; init; }

    [JsonPropertyName("ask")]
    public required string Ask { get; init; }

    [JsonPropertyName("last")]
    public required string Last { get; init; }

    [JsonPropertyName("timestamp")]
    public required string Timestamp { get; init; }

    [JsonPropertyName("rules")]
    public required MarketSnapshotRules Rules { get; init; }

    [JsonPropertyName("status")]
    public required string Status { get; init; }
}
