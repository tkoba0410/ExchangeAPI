using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
namespace ExchangeApi.Common.Dtos;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt);
