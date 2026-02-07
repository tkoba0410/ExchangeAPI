using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeOrderSummaryNormalized(
    OrderId Id,
    Symbol Symbol,
    FreeText AccountId,
    decimal Amount,
    decimal? Price,
    FreeText State,
    FreeText Type,
    FreeText? ClientOrderId,
    DateTimeOffset CreatedAt,
    decimal FilledAmount);
