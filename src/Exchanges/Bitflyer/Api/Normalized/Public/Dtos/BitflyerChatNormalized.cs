using System;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerChatNormalized(
    string? Nickname,
    string? Message,
    DateTimeOffset? Timestamp);
