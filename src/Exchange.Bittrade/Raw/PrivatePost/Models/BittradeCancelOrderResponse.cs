using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] string OrderId);
