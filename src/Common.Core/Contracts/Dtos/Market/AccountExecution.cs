using System;

namespace ExchangeApi.Contracts.Dtos;

/// <summary>
/// 口座の約定履歴。
/// </summary>
public sealed record AccountExecution(
    string ProductCode,
    long Id,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    string? ChildOrderAcceptanceId = null,
    decimal? Commission = null,
    decimal? Pnl = null,
    string? Liquidity = null);
