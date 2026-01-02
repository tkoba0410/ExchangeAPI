using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(OrderIdJsonConverter))] RawOrderId RawOrderId);
