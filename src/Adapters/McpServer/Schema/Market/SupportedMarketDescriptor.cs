using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Market;

public sealed class SupportedMarketDescriptor
{
    [JsonPropertyName("venue")]
    public required string Venue { get; init; }

    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("capabilities")]
    public required IReadOnlyList<string> Capabilities { get; init; }
}
