using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed record BitflyerChat(
    [property: JsonPropertyName("nickname")] string? Nickname,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("date")] DateTimeOffset? Date);
