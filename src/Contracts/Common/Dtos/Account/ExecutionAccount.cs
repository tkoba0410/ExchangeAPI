using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos.Account;

/// <summary>
/// 口座の約定履歴（シンボル＋注文IDベース）。
/// </summary>
public sealed record ExecutionAccount(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    Price Price,
    Size Size,
    DateTimeOffset ExecutedAt,
    decimal? Commission = null,
    decimal? Pnl = null,
    string? Liquidity = null);
