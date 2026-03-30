using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Market;

public sealed class ListMarketsResponse
{
    [JsonPropertyName("markets")]
    public required IReadOnlyList<SupportedMarketDescriptor> Markets { get; init; }
}
