using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Exchange.Bitflyer.Raw;

/// <summary>/v1/me/sendparentorder リクエスト DTO。</summary>
public sealed class BitflyerSendParentOrderRequest
{
    [JsonPropertyName("order_method")] public OrderMethod OrderMethod { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TimeInForce? TimeInForce { get; init; }

    [JsonPropertyName("parameters")] public IReadOnlyList<BitflyerParentOrderParameter> Parameters { get; init; } = new List<BitflyerParentOrderParameter>();
}

public sealed class BitflyerParentOrderParameter
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; }
    [JsonPropertyName("condition_type")] public ConditionType ConditionType { get; init; }
    [JsonPropertyName("side")] public Side Side { get; init; }
    [JsonPropertyName("size")] public decimal Size { get; init; }

    [JsonPropertyName("price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; init; }

    [JsonPropertyName("trigger_price")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? TriggerPrice { get; init; }

    [JsonPropertyName("offset")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Offset { get; init; }
}

public sealed class BitflyerSendParentOrderResponse
{
    [JsonPropertyName("parent_order_acceptance_id")]
    public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}
