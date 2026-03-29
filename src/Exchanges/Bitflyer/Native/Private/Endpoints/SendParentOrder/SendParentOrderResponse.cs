using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;

public sealed class SendParentOrderResponse
{
    [JsonPropertyName("parent_order_acceptance_id")]
    public required string ParentOrderAcceptanceId { get; init; }
}
