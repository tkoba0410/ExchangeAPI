using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Wire.Public;

public sealed record Chat(
    [property: JsonPropertyName("nickname")] string? Nickname,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("date")] DateTimeOffset? Date);
