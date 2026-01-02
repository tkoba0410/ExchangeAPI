using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>
/// /v1/me/cancelchildorder のリクエスト DTO。
/// </summary>
public sealed class CancelChildOrderRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }

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
