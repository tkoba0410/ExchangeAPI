using System;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

public sealed class ChildOrderResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("child_order_id")] public string ChildOrderId { get; init; } = string.Empty;
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("child_order_type")] public string ChildOrderType { get; init; } = string.Empty;
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("average_price")] public decimal AveragePrice { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("child_order_state")] public string ChildOrderStatusState { get; init; } = string.Empty;
    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("child_order_date")] public DateTimeOffset ChildOrderDate { get; init; }
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
    [JsonPropertyName("outstanding_size")] public decimal OutstandingSize { get; init; }
    [JsonPropertyName("cancel_size")] public decimal CancelSize { get; init; }
    [JsonPropertyName("executed_size")] public decimal ExecutedSize { get; init; }
    [JsonPropertyName("total_commission")] public decimal TotalCommission { get; init; }
    [JsonPropertyName("time_in_force")] public string? TimeInForce { get; init; }
}
