using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed class BitflyerChildOrderResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("child_order_id")] public string ChildOrderId { get; init; } = string.Empty;
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("child_order_type")] public ChildOrderType ChildOrderType { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("average_price")] public decimal AveragePrice { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("child_order_state")] public string ChildOrderState { get; init; } = string.Empty;
    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("child_order_date")] public DateTimeOffset ChildOrderDate { get; init; }
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
    [JsonPropertyName("outstanding_size")] public decimal OutstandingSize { get; init; }
    [JsonPropertyName("cancel_size")] public decimal CancelSize { get; init; }
    [JsonPropertyName("executed_size")] public decimal ExecutedSize { get; init; }
    [JsonPropertyName("total_commission")] public decimal TotalCommission { get; init; }
    [JsonPropertyName("time_in_force")] public TimeInForce? TimeInForce { get; init; }
}
