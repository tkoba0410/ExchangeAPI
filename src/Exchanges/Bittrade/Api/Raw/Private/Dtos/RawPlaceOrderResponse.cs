using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record RawPlaceOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string OrderId);
