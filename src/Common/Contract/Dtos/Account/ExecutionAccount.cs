using System;
using System.Globalization;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>
/// 口座の約定履歴（シンボル＋注文IDベース）。
/// </summary>
public sealed record ExecutionAccount(
    Symbol Symbol,
    string OrderId,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    string? Liquidity = null);
