using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeBalanceData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("list")] IReadOnlyList<BittradeBalanceEntry> List);
