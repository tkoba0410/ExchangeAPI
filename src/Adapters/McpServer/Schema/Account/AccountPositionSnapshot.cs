using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Account;

public sealed class AccountPositionSnapshot
{
    [JsonPropertyName("symbol")]
    public required string Symbol { get; init; }

    [JsonPropertyName("side")]
    public required string Side { get; init; }

    [JsonPropertyName("size")]
    public required string Size { get; init; }

    [JsonPropertyName("avgPrice")]
    public required string AvgPrice { get; init; }
}
