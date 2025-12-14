using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>市場全体の約定（歩み値）。</summary>
public sealed record ExecutionMarket(
    string ProductCode,
    long Id,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt);
