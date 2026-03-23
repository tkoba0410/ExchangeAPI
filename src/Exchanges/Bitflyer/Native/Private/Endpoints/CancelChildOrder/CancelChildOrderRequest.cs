using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelChildOrder;

public sealed class CancelChildOrderRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("child_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderId { get; init; }

    [JsonPropertyName("child_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderAcceptanceId { get; init; }
}
