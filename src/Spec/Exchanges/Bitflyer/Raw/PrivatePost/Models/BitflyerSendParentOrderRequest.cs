using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Raw;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

/// <summary>/v1/me/sendparentorder リクエスト DTO。</summary>
public sealed class CreateParentOrderRequest
{
    [JsonPropertyName("order_method")] public string OrderMethod { get; init; } = string.Empty;

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TimeInForce { get; init; }

    [JsonPropertyName("parameters")] public IReadOnlyList<ParentOrderParameter> Parameters { get; init; } = new List<ParentOrderParameter>();
}

public sealed class ParentOrderParameter
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("condition_type")] public string ConditionType { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
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

public sealed class CreateParentOrderResponse
{
    [JsonPropertyName("parent_order_acceptance_id")]
    public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}
