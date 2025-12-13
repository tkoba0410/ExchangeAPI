using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

/// <summary>/v1/me/cancelparentorder リクエスト DTO。</summary>
public sealed class BitflyerCancelParentOrderRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }

    [JsonPropertyName("parent_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderId { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderAcceptanceId { get; init; }
}
