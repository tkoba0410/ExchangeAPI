using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;

public sealed class GetExecutionsRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }

    [JsonPropertyName("child_order_id")]
    public string? ChildOrderId { get; init; }

    [JsonPropertyName("child_order_acceptance_id")]
    public string? ChildOrderAcceptanceId { get; init; }
}
