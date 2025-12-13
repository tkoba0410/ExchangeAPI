using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeAccount(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("subtype")] string? SubType,
    [property: JsonPropertyName("state")] string State);
