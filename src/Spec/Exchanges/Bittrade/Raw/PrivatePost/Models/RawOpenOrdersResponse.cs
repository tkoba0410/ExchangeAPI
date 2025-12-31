using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawOpenOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawOrderSummary>? Data);
