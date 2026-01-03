using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed class RawCancelChildOrderRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }

    [JsonPropertyName("child_order_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderId { get; init; }

    [JsonPropertyName("child_order_acceptance_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ChildOrderAcceptanceId { get; init; }
}
