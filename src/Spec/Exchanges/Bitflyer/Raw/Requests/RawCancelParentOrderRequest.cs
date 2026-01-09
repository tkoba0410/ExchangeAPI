using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed class RawCancelParentOrderRequest
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;

    [JsonPropertyName("parent_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderId { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderAcceptanceId { get; init; }
}
