using System;
using Common.Enums;
namespace Common.Dtos;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt);
