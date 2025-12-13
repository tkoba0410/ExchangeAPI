using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>
/// 親注文一覧 (/v1/me/getparentorders) のレスポンス DTO。
/// </summary>
public sealed class BitflyerParentOrderResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("parent_order_id")] public string ParentOrderId { get; init; } = string.Empty;
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }

    /// <summary>親注文種別。ドキュメント例では STOP / IFD などが返る。</summary>
    [JsonPropertyName("parent_order_type")] public ParentOrderType ParentOrderType { get; init; }

    [JsonPropertyName("price")] public decimal Price { get; init; }
    [JsonPropertyName("average_price")] public decimal AveragePrice { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }

    [JsonPropertyName("parent_order_state")] public string ParentOrderState { get; init; } = string.Empty;

    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("parent_order_date")] public DateTimeOffset ParentOrderDate { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")] public string ParentOrderAcceptanceId { get; init; } = string.Empty;
    [JsonPropertyName("outstanding_size")] public decimal OutstandingSize { get; init; }
    [JsonPropertyName("cancel_size")] public decimal CancelSize { get; init; }
    [JsonPropertyName("executed_size")] public decimal ExecutedSize { get; init; }
    [JsonPropertyName("total_commission")] public decimal TotalCommission { get; init; }
}
