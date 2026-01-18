using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading;

public sealed record BittradeOpenOrder(
    Symbol Symbol,
    OrderKey Key,
    Side Side,
    OrderType OrderType,
    Size Size,
    Size OutstandingSize,
    Size ExecutedSize,
    Price? Price,
    DateTimeOffset? OrderedAt = null,
    DateTimeOffset? UpdatedAt = null,
    Price? StopPrice = null,
    string? Status = null,
    string? ExchangeOrderId = null,
    string? AcceptanceId = null);
