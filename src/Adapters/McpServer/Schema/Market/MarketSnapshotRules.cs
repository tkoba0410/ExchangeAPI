using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Market;

public sealed class MarketSnapshotRules
{
    [JsonPropertyName("minSize")]
    public required string MinSize { get; init; }

    [JsonPropertyName("sizeStep")]
    public required string SizeStep { get; init; }

    [JsonPropertyName("priceStep")]
    public required string PriceStep { get; init; }
}
