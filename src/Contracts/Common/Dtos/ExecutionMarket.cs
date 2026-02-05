using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    Symbol Symbol,
    OrderId OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt);
