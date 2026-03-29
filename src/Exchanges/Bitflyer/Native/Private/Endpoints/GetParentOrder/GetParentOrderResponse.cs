using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;

public sealed class GetParentOrderResponse
{
    [JsonPropertyName("id")]
    public required long Id { get; init; }
    [JsonPropertyName("parent_order_id")]
    public required string ParentOrderId { get; init; }
    [JsonPropertyName("order_method")]
    public required BitflyerOrderMethod OrderMethod { get; init; }
    [JsonPropertyName("expire_date")]
    public required DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("time_in_force")]
    public required BitflyerTimeInForce TimeInForce { get; init; }
    [JsonPropertyName("parameters")]
    public required IReadOnlyList<GetParentOrderParameter> Parameters { get; init; }
    [JsonPropertyName("parent_order_acceptance_id")]
    public required string ParentOrderAcceptanceId { get; init; }
}

public sealed class GetParentOrderParameter
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }
    [JsonPropertyName("condition_type")]
    public required BitflyerConditionType ConditionType { get; init; }
    [JsonPropertyName("side")]
    public required BitflyerOrderSide Side { get; init; }
    [JsonPropertyName("price")]
    public required decimal Price { get; init; }
    [JsonPropertyName("size")]
    public required decimal Size { get; init; }
    [JsonPropertyName("trigger_price")]
    public required decimal TriggerPrice { get; init; }
    [JsonPropertyName("offset")]
    public required decimal Offset { get; init; }
}
