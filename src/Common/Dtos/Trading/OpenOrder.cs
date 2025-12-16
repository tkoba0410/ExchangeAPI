using System;
using Common.Enums;
namespace Common.Dtos;

/// <summary>
/// オープンな子注文の概要（受付IDなし、Exchange/時刻を含む）。
/// </summary>
public sealed record OpenOrder(
    ExchangeCode ExchangeCode,
    Symbol Symbol,
    string OrderId,
    Side Side,
    OrderType OrderType,
    decimal Size,
    decimal OutstandingSize,
    decimal ExecutedSize,
    decimal? Price,
    DateTimeOffset? OrderedAt = null,
    DateTimeOffset? UpdatedAt = null,
    decimal? StopPrice = null,
    string? Status = null);
