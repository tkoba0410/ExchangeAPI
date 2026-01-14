using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

public sealed class RawGetParentOrderResponse
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
