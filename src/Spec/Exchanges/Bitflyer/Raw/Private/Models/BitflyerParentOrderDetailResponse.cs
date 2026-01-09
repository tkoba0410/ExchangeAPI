using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private;

public sealed class ParentOrderDetailResponse
{
    [JsonPropertyName("id")] public long Id { get; init; }
    [JsonPropertyName("parent_order_id")] public string ParentOrderId { get; init; } = string.Empty;
    [JsonPropertyName("order_method")] public string OrderMethod { get; init; } = string.Empty;
    [JsonPropertyName("expire_date")] public DateTimeOffset ExpireDate { get; init; }
    [JsonPropertyName("time_in_force")] public string TimeInForce { get; init; } = string.Empty;
    [JsonPropertyName("parameters")] public IReadOnlyList<ParentOrderParameterResponse> Parameters { get; init; }
        = new List<ParentOrderParameterResponse>();
    [JsonPropertyName("parent_order_acceptance_id")] public string ParentOrderAcceptanceId { get; init; } = string.Empty;
}

public sealed class ParentOrderParameterResponse
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("condition_type")] public string ConditionType { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("price")] public decimal? Price { get; init; }
    [JsonPropertyName("size")] public decimal? Size { get; init; }
    [JsonPropertyName("trigger_price")] public decimal? TriggerPrice { get; init; }
    [JsonPropertyName("offset")] public decimal? Offset { get; init; }
}
