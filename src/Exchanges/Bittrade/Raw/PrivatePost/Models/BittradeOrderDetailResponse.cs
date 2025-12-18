using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeOrderDetailResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeOrderDetail? Data);
