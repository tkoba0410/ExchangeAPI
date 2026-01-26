using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos.Account;

public sealed record BitflyerExecutionAccountNormalized(
    Symbol Symbol,
    string OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    string? Liquidity = null);
