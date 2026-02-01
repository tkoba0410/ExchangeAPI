using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record RawBalanceData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("list")] IReadOnlyList<RawBalanceEntry> List);
