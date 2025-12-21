using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record MatchResultsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<MatchResultEntry>? Data);

public sealed record OrderMatchResultsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<MatchResultEntry>? Data);

public sealed record MatchResultEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("order-id")]
    [property: JsonConverter(typeof(OrderIdJsonConverter))] OrderId OrderId,
    [property: JsonPropertyName("match-id")] long MatchId,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] Symbol Symbol,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("filled-amount")] decimal FilledAmount,
    [property: JsonPropertyName("filled-fees")] decimal FilledFees,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset CreatedAt);
