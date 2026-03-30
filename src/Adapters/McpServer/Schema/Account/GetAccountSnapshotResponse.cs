using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Account;

public sealed class GetAccountSnapshotResponse
{
    [JsonPropertyName("permissionModel")]
    public required string PermissionModel { get; init; }

    [JsonPropertyName("balance")]
    public required IReadOnlyDictionary<string, string> Balance { get; init; }

    [JsonPropertyName("positions")]
    public required IReadOnlyList<AccountPositionSnapshot> Positions { get; init; }

    [JsonPropertyName("openOrdersSummary")]
    public required OpenOrdersSummary OpenOrdersSummary { get; init; }

    [JsonPropertyName("margin")]
    public required AccountMarginSnapshot Margin { get; init; }

    [JsonPropertyName("accountReadiness")]
    public required string AccountReadiness { get; init; }
}
