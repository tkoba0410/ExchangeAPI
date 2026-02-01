using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawRetailOrderDetailRequest(
    [property: JsonPropertyName("orderId")] string OrderId);
