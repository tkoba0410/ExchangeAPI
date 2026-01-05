using System.Text.Json.Serialization;
using ExchangeApi.Spec.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string OrderId);
