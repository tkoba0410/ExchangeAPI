using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Contracts.Dtos.Market;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt);
