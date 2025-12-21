using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(OrderIdJsonConverter))] OrderId OrderId);
