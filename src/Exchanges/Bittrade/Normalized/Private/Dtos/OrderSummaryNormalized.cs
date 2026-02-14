using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record OrderSummaryNormalized(
    OrderId OrderId,
    Symbol Symbol,
    AccountId AccountId,
    decimal Amount,
    decimal? Price,
    Closed<ExchangeOrderState> State,
    Closed<ExchangeOrderType> Type,
    FreeText? ClientOrderId,
    DateTimeOffset CreatedAt,
    decimal FilledAmount);
