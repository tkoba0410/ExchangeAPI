using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

/// <summary>
/// /v1/me/sendparentorder のリクエスト DTO。
/// </summary>
public sealed class CreateParentOrderRequest
{
    [JsonPropertyName("order_method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OrderMethod { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TimeInForce { get; init; }

    [JsonPropertyName("parameters")] public IReadOnlyList<CreateParentOrderParameter> Parameters { get; init; }
        = new List<CreateParentOrderParameter>();
}

public sealed class CreateParentOrderParameter
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
