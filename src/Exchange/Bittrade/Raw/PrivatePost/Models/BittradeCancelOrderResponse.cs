using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] string OrderId);
