using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record ExecutionsPublicResponse(IReadOnlyList<ExecutionsPublicItem> Items);

public sealed record ExecutionsPublicItem(
    Symbol Symbol,
    OrderId OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt);
