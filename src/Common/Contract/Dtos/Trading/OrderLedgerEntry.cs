using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>注文台帳の1エントリ。</summary>
public sealed record OrderLedgerEntry(
    string LocalId,
    ExchangeCode Exchange,
    Symbol Symbol,
    Side Side,
    OrderType OrderType,
    decimal Size,
    decimal? Price,
    decimal? TriggerPrice,
    string? ServerOrderId,
    OrderState Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string? LastError = null);
