using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;

public sealed class SendChildOrderResponse
{
    [JsonPropertyName("child_order_acceptance_id")]
    public required string ChildOrderAcceptanceId { get; init; }
}
