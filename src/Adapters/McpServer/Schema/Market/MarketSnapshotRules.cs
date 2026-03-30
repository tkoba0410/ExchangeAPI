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

    [JsonPropertyName("minSizeSourceKind")]
    public required string MinSizeSourceKind { get; init; }

    [JsonPropertyName("minSizeSourceRef")]
    public required string MinSizeSourceRef { get; init; }

    [JsonPropertyName("sizeStepSourceKind")]
    public required string SizeStepSourceKind { get; init; }

    [JsonPropertyName("sizeStepSourceRef")]
    public required string SizeStepSourceRef { get; init; }

    [JsonPropertyName("priceStepSourceKind")]
    public required string PriceStepSourceKind { get; init; }

    [JsonPropertyName("priceStepSourceRef")]
    public required string PriceStepSourceRef { get; init; }
}
