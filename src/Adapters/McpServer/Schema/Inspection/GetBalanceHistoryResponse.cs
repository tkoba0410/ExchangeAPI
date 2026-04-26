using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetBalanceHistoryResponse
{
    [JsonPropertyName("items")]
    public required IReadOnlyList<BalanceHistoryItem> Items { get; init; }
}

public sealed class BalanceHistoryItem
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("tradeDate")]
    public required DateTimeOffset TradeDate { get; init; }

    [JsonPropertyName("eventDate")]
    public required DateTimeOffset EventDate { get; init; }

    [JsonPropertyName("productCode")]
    public string? ProductCode { get; init; }

    [JsonPropertyName("currencyCode")]
    public required string CurrencyCode { get; init; }

    [JsonPropertyName("tradeType")]
    public required string TradeType { get; init; }

    [JsonPropertyName("price")]
    public required string Price { get; init; }

    [JsonPropertyName("amount")]
    public required string Amount { get; init; }

    [JsonPropertyName("quantity")]
    public required string Quantity { get; init; }

    [JsonPropertyName("commission")]
    public required string Commission { get; init; }

    [JsonPropertyName("balance")]
    public required string Balance { get; init; }

    [JsonPropertyName("orderId")]
    public string? OrderId { get; init; }
}
