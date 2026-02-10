using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record PostOrdersBatchCancelOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] PostOrdersBatchCancelOpenOrdersItem? Data);

public sealed record PostOrdersBatchCancelOpenOrdersItem(
    [property: JsonPropertyName("success-count")] int SuccessCount,
    [property: JsonPropertyName("failed-count")] int FailedCount,
    [property: JsonPropertyName("next-id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string? NextId);
