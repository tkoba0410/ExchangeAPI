using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;

public sealed record BittradeExecutionAccountNormalized(
    Symbol Symbol,
    OrderId OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    FreeText? Liquidity = null);
