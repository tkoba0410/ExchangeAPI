using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;

/// <summary>
/// /v1/me/cancelchildorder のリクエスト DTO。
/// </summary>
public sealed class CancelChildOrderRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; } = ProductCode.Empty;

    /// <summary>
    /// child_order_acceptance_id を優先して使用する。
    /// </summary>
    [JsonPropertyName("child_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("child_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? ChildOrderId { get; init; }
}
