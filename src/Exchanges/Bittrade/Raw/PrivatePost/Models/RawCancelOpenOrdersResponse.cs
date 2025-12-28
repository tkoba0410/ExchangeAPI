using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] RawCancelOpenOrdersResult? Data);

public sealed record RawCancelOpenOrdersResult(
    [property: JsonPropertyName("success-count")] int SuccessCount,
    [property: JsonPropertyName("failed-count")] int FailedCount,
    [property: JsonPropertyName("next-id")]
    [property: JsonConverter(typeof(CursorIdJsonConverter))] RawCursorId? NextId);
