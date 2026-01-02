using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>/v1/me/cancelparentorder リクエスト DTO。</summary>
public sealed class CancelParentOrderRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }

    [JsonPropertyName("parent_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderId { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderAcceptanceId { get; init; }
}
