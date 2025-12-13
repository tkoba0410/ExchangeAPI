using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PrivatePost.Models;

/// <summary>
/// /v1/me/cancelchildorder のリクエスト DTO。
/// </summary>
public sealed class BitflyerCancelChildOrderRequest
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;

    /// <summary>
    /// child_order_acceptance_id を優先して使用する。
    /// </summary>
    [JsonPropertyName("child_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("child_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderId { get; init; }
}
