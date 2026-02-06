using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record GetRetailAccountBalanceResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] IReadOnlyList<GetRetailAccountBalanceEntry>? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);

public sealed record GetRetailAccountBalanceEntry(
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("balance")] string? Balance,
    [property: JsonPropertyName("available")] string? Available,
    [property: JsonPropertyName("frozen")] string? Frozen);
