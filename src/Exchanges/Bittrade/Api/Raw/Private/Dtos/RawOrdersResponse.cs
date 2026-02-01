using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record RawOrdersResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawOrderSummary>? Data);
