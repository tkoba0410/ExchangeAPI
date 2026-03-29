using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Market;

public sealed class GetMarketSnapshotRequest
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }
}
