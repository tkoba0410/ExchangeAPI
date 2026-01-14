using System.Text.Json.Serialization;
using ExchangeApi.Contracts.Common.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawPlaceOrderResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string OrderId);
