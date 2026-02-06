using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record PostOrdersBatchCancelResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] object? Data);
