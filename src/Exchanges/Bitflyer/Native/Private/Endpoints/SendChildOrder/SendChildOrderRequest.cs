using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendChildOrder;

public sealed class SendChildOrderRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("child_order_type")]
    public required string ChildOrderType { get; init; }

    [JsonPropertyName("side")]
    public required string Side { get; init; }

    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    [JsonPropertyName("size")]
    public required decimal Size { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TimeInForce { get; init; }
}
