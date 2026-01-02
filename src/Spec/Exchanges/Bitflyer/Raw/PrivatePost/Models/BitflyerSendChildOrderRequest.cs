using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>
/// /v1/me/sendchildorder のリクエスト DTO。
/// </summary>
public sealed class CreateChildOrderRequest
{
    [JsonPropertyName("product_code")] public RawProductCode ProductCode { get; init; }
    [JsonPropertyName("child_order_type")] public ChildOrderType ChildOrderType { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }

    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeInForce? TimeInForce { get; init; }

    [JsonPropertyName("trigger_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TriggerPrice { get; init; }
}
