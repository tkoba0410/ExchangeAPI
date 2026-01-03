using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawMatchResultsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawMatchResultEntry>? Data);

public sealed record RawOrderMatchResultsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawMatchResultEntry>? Data);

public sealed record RawMatchResultEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(MatchResultIdJsonConverter))] RawMatchResultId Id,
    [property: JsonPropertyName("order-id")]
    [property: JsonConverter(typeof(OrderIdJsonConverter))] RawOrderId RawOrderId,
    [property: JsonPropertyName("match-id")]
    [property: JsonConverter(typeof(MatchIdJsonConverter))] RawMatchId RawMatchId,
    [property: JsonPropertyName("symbol")]
    [property: JsonConverter(typeof(SymbolJsonConverter))] RawSymbol RawSymbol,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("filled-amount")] decimal FilledAmount,
    [property: JsonPropertyName("filled-fees")] decimal FilledFees,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset CreatedAt);
