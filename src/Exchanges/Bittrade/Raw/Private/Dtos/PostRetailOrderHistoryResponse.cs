using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record PostRetailOrderHistoryResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] IReadOnlyList<RawRetailOrderEntry>? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);
