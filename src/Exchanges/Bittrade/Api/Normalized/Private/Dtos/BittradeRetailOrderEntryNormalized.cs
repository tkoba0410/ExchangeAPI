using System;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;

public sealed record BittradeRetailOrderEntryNormalized(
    OrderId Id,
    Symbol Symbol,
    int Type,
    decimal? Price,
    decimal? Amount,
    decimal? CashAmount,
    int? Status,
    DateTimeOffset? CreatedAt);
