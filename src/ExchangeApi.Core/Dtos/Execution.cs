using System;

namespace ExchangeApi.Core.Dtos;

/// <summary>
/// 約定履歴。
/// </summary>
public sealed record Execution(
    string ProductCode,
    long Id,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    string? ChildOrderAcceptanceId = null);
