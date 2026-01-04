using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;

/// <summary>
/// 親注文の詳細 (/v1/me/getparentorder) のレスポンス DTO。
/// </summary>
public sealed class ParentOrderDetailResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("parent_order_id")] public string ParentOrderId { get; init; } = string.Empty;
    [JsonPropertyName("order_method")] public string OrderMethod { get; init; } = string.Empty;
    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("time_in_force")] public string TimeInForce { get; init; } = string.Empty;

    [JsonPropertyName("parameters")]
    public IReadOnlyList<ParentOrderDetailParameter> Parameters { get; init; } =
        Array.Empty<ParentOrderDetailParameter>();

    [JsonPropertyName("parent_order_acceptance_id")]
    public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}

public sealed class ParentOrderDetailParameter
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("condition_type")] public string ConditionType { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("trigger_price")] public decimal TriggerPrice { get; init; }
    [JsonPropertyName("offset")] public decimal Offset { get; init; }
}
