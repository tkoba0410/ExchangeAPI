using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record ExecutionAccountNormalized(
    long Id,
    ProductCode ProductCode,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt,
    AcceptanceId? ChildOrderAcceptanceId = null);
