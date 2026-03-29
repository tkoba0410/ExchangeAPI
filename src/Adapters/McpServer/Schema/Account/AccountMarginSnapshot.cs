using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Account;

public sealed class AccountMarginSnapshot
{
    [JsonPropertyName("derivedAvailable")]
    public required string? DerivedAvailable { get; init; }
}
