using System;

namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 市場全体の約定（歩み値）。
/// </summary>
public sealed record MarketExecution(
    string ProductCode,
    long Id,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt);
