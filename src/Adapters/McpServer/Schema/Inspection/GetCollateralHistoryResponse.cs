using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetCollateralHistoryResponse
{
    [JsonPropertyName("items")]
    public required IReadOnlyList<CollateralHistoryItem> Items { get; init; }
}

public sealed class CollateralHistoryItem
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("currencyCode")]
    public required string CurrencyCode { get; init; }

    [JsonPropertyName("change")]
    public required string Change { get; init; }

    [JsonPropertyName("amount")]
    public required string Amount { get; init; }

    [JsonPropertyName("reasonCode")]
    public required string ReasonCode { get; init; }

    [JsonPropertyName("date")]
    public required DateTimeOffset Date { get; init; }
}
