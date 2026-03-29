using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.SendParentOrder;

public sealed class SendParentOrderRequest
{
    [JsonPropertyName("order_method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BitflyerOrderMethod? OrderMethod { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BitflyerTimeInForce? TimeInForce { get; init; }

    [JsonPropertyName("parameters")]
    public required IReadOnlyList<SendParentOrderParameter> Parameters { get; init; }
}

public sealed class SendParentOrderParameter
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("condition_type")]
    public required BitflyerConditionType ConditionType { get; init; }

    [JsonPropertyName("side")]
    public required BitflyerOrderSide Side { get; init; }

    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    [JsonPropertyName("size")]
    public required decimal Size { get; init; }

    [JsonPropertyName("trigger_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TriggerPrice { get; init; }

    [JsonPropertyName("offset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Offset { get; init; }
}
