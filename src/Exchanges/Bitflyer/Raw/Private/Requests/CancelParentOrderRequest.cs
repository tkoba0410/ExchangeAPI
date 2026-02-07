using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;

/// <summary>
/// /v1/me/cancelparentorder のリクエスト DTO。
/// </summary>
public sealed class CancelParentOrderRequest
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; } = ProductCode.Empty;

    [JsonPropertyName("parent_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? ParentOrderAcceptanceId { get; init; }

    [JsonPropertyName("parent_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? ParentOrderId { get; init; }
}
