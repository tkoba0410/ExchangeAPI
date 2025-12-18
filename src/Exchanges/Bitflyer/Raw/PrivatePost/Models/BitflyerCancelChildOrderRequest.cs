using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// /v1/me/cancelchildorder のリクエスト DTO。
/// </summary>
public sealed class BitflyerCancelChildOrderRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }

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
