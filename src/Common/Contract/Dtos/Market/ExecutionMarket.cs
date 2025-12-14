using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    Symbol Symbol,
    string OrderId,
    Side Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt);
