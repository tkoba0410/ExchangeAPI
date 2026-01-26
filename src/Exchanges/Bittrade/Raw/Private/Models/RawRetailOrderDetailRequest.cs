using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawRetailOrderDetailRequest(
    [property: JsonPropertyName("orderId")] string OrderId);
