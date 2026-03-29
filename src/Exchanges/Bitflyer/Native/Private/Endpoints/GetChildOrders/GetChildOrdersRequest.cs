using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;

public sealed class GetChildOrdersRequest
{
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; init; }

    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }

    [JsonPropertyName("child_order_state")]
    public BitflyerOrderState? ChildOrderState { get; init; }

    [JsonPropertyName("child_order_id")]
    public string? ChildOrderId { get; init; }

    [JsonPropertyName("child_order_acceptance_id")]
    public string? ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("parent_order_id")]
    public string? ParentOrderId { get; init; }
}
