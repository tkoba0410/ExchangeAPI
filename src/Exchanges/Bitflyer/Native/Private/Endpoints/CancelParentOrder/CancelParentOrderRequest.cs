using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelParentOrder;

public sealed class CancelParentOrderRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("parent_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderId { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ParentOrderAcceptanceId { get; init; }
}
