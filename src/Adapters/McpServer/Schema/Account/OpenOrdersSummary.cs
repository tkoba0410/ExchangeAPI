using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Account;

public sealed class OpenOrdersSummary
{
    [JsonPropertyName("count")]
    public required int Count { get; init; }
}
