using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetChildOrdersResponse
{
    [JsonPropertyName("orders")]
    public required IReadOnlyList<ChildOrderItem> Orders { get; init; }
}

public sealed class ChildOrderItem
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }

    [JsonPropertyName("childOrderId")]
    public required string ChildOrderId { get; init; }

    [JsonPropertyName("productCode")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("side")]
    public required string Side { get; init; }

    [JsonPropertyName("childOrderType")]
    public required string ChildOrderType { get; init; }

    [JsonPropertyName("price")]
    public required string Price { get; init; }

    [JsonPropertyName("averagePrice")]
    public required string AveragePrice { get; init; }

    [JsonPropertyName("size")]
    public required string Size { get; init; }

    [JsonPropertyName("childOrderState")]
    public required string ChildOrderState { get; init; }

    [JsonPropertyName("expireDate")]
    public required DateTimeOffset ExpireDate { get; init; }

    [JsonPropertyName("childOrderDate")]
    public required DateTimeOffset ChildOrderDate { get; init; }

    [JsonPropertyName("childOrderAcceptanceId")]
    public required string ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("outstandingSize")]
    public required string OutstandingSize { get; init; }

    [JsonPropertyName("cancelSize")]
    public required string CancelSize { get; init; }

    [JsonPropertyName("executedSize")]
    public required string ExecutedSize { get; init; }

    [JsonPropertyName("totalCommission")]
    public required string TotalCommission { get; init; }

    [JsonPropertyName("timeInForce")]
    public required string TimeInForce { get; init; }
}
