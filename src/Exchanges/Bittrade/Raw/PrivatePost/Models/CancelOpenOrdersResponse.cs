using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CancelOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] CancelOpenOrdersResult? Data);

public sealed record CancelOpenOrdersResult(
    [property: JsonPropertyName("success-count")] int SuccessCount,
    [property: JsonPropertyName("failed-count")] int FailedCount,
    [property: JsonPropertyName("next-id")] long? NextId);
