using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record RetailOrderEntryNormalized(
    OrderId Id,
    Symbol Symbol,
    int Type,
    decimal? Price,
    decimal? Amount,
    decimal? CashAmount,
    int? Status,
    DateTimeOffset? CreatedAt);
