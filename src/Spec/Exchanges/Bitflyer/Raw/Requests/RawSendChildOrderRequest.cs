using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed class RawSendChildOrderRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }
    [JsonPropertyName("child_order_type")] public string ChildOrderType { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("size")] public decimal Size { get; init; }

    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TimeInForce { get; init; }

    [JsonPropertyName("trigger_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TriggerPrice { get; init; }
}
