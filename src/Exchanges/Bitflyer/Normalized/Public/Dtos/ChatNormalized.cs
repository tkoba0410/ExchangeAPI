using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record ChatNormalized(
    FreeText? Nickname,
    FreeText? Message,
    DateTimeOffset? Timestamp);
