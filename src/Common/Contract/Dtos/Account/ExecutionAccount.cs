using System;
using System.Globalization;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>
/// 口座の約定履歴（シンボル＋注文IDベース）。
/// </summary>
public sealed record ExecutionAccount(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    string? Liquidity = null);
