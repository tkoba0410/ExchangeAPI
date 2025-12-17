using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradePlaceOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long OrderId);
