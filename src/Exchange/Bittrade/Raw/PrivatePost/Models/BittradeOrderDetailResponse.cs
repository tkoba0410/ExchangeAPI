using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeOrderDetailResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeOrderDetail? Data);
