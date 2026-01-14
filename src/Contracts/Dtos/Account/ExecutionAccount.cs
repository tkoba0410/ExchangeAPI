using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Contracts.Dtos.Account;

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
