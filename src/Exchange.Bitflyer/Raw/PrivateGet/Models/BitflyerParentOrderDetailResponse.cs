using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// 親注文の詳細 (/v1/me/getparentorder) のレスポンス DTO。
/// </summary>
public sealed class BitflyerParentOrderDetailResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("parent_order_id")] public string ParentOrderId { get; init; } = string.Empty;
    [JsonPropertyName("order_method")] public OrderMethod OrderMethod { get; init; }
    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("time_in_force")] public TimeInForce TimeInForce { get; init; }

    [JsonPropertyName("parameters")]
    public IReadOnlyList<BitflyerParentOrderDetailParameter> Parameters { get; init; } =
        Array.Empty<BitflyerParentOrderDetailParameter>();

    [JsonPropertyName("parent_order_acceptance_id")]
    public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}

public sealed class BitflyerParentOrderDetailParameter
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("condition_type")] public ChildOrderType ConditionType { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }
    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("trigger_price")] public decimal TriggerPrice { get; init; }
    [JsonPropertyName("offset")] public decimal Offset { get; init; }
}
