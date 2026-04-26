using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetCollateralAccountsResponse
{
    [JsonPropertyName("accounts")]
    public required IReadOnlyList<CollateralAccountItem> Accounts { get; init; }
}

public sealed class CollateralAccountItem
{
    [JsonPropertyName("currencyCode")]
    public required string CurrencyCode { get; init; }

    [JsonPropertyName("amount")]
    public required string Amount { get; init; }
}
