using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Dtos;

public sealed record BitflyerChatNormalized(
    FreeText? Nickname,
    FreeText? Message,
    DateTimeOffset? Timestamp);
