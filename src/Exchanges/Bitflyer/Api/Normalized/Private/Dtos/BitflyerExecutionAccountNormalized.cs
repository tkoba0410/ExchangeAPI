using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerExecutionAccountNormalized(
    Symbol Symbol,
    OrderId OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    FreeText? Liquidity = null);
