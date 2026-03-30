using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Account;

public sealed class GetAccountSnapshotRequest
{
    [JsonPropertyName("venue")]
    public required string Venue { get; init; }

    [JsonPropertyName("accountContext")]
    public required string AccountContext { get; init; }
}
