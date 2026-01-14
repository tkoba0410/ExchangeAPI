using System;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerChatNormalized(
    string? Nickname,
    string? Message,
    DateTimeOffset? Timestamp);
