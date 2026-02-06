using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;

public sealed class SendChildOrderResponse
{
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
}
