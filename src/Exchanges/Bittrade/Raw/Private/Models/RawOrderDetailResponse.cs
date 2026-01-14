using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawOrderDetailResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] RawOrderDetail? Data);
