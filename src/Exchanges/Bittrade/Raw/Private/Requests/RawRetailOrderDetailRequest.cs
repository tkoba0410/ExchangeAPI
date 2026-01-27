using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;

public sealed record RawRetailOrderDetailRequest(
    [property: JsonPropertyName("orderId")] string OrderId);
