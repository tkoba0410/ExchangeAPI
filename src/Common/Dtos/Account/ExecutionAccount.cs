using System;
using System.Globalization;
using ExchangeApi.Common.Enums;
namespace ExchangeApi.Common.Dtos;

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
