using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

public sealed class RawSendChildOrderResponse
{
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
}
