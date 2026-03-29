using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;

public sealed class GetParentOrderRequest
{
    [JsonPropertyName("parent_order_id")]
    public string? ParentOrderId { get; init; }
    [JsonPropertyName("parent_order_acceptance_id")]
    public string? ParentOrderAcceptanceId { get; init; }
}
