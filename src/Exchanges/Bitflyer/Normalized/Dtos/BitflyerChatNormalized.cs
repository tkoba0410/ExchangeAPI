using System;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerChatNormalized(
    string? Nickname,
    string? Message,
    DateTimeOffset? Timestamp);
