using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

public sealed class RawSendParentOrderResponse
{
    [JsonPropertyName("parent_order_acceptance_id")] public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}
