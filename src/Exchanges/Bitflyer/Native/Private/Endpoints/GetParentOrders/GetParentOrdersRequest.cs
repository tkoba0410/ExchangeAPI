using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;

public sealed class GetParentOrdersRequest
{
    [JsonPropertyName("product_code")]
    public string? ProductCode { get; init; }
    [JsonPropertyName("count")]
    public int? Count { get; init; }
    [JsonPropertyName("before")]
    public long? Before { get; init; }
    [JsonPropertyName("after")]
    public long? After { get; init; }
    [JsonPropertyName("parent_order_state")]
    public BitflyerOrderState? ParentOrderState { get; init; }
}
