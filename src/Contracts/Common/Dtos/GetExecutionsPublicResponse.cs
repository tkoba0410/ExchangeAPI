using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetExecutionsPublicResponse(IReadOnlyList<GetExecutionsPublicItem> Items);

public sealed record GetExecutionsPublicItem(
    Symbol Symbol,
    OrderId OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt);
