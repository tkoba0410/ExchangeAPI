using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Requests;

/// <summary>
/// /v1/me/sendparentorder のリクエスト DTO。
/// </summary>
public sealed class SendParentOrderRequest
{
    [JsonPropertyName("order_method")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? OrderMethod { get; init; }

    [JsonPropertyName("minute_to_expire")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinuteToExpire { get; init; }

    [JsonPropertyName("time_in_force")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? TimeInForce { get; init; }

    [JsonPropertyName("parameters")] public IReadOnlyList<CreateParentOrderParameter> Parameters { get; init; }
        = new List<CreateParentOrderParameter>();
}

public sealed class CreateParentOrderParameter
{
    [JsonPropertyName("product_code")] public ProductCode ProductCode { get; init; } = ProductCode.Empty;
    [JsonPropertyName("condition_type")] public FreeText ConditionType { get; init; } = FreeText.Empty;
    [JsonPropertyName("side")] public FreeText Side { get; init; } = FreeText.Empty;
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
