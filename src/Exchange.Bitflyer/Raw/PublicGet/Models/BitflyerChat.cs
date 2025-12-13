using System;
using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PublicGet.Models;

public sealed record BitflyerChat(
    [property: JsonPropertyName("nickname")] string? Nickname,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("date")] DateTimeOffset? Date);
