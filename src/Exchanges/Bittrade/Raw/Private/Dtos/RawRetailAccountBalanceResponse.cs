using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record RawRetailAccountBalanceResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] IReadOnlyList<RawRetailAccountBalanceEntry>? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);

public sealed record RawRetailAccountBalanceEntry(
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("balance")] string? Balance,
    [property: JsonPropertyName("available")] string? Available,
    [property: JsonPropertyName("frozen")] string? Frozen);
