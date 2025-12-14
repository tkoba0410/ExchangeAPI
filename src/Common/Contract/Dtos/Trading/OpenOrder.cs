using System;
using Common.Contract.Enums;

namespace Common.Contract.Dtos;

/// <summary>
/// オープンな子注文の概要（受付IDなし、Exchange/時刻を含む）。
/// </summary>
public sealed record OpenOrder(
    ExchangeCode Exchange,
    string ProductCode,
    string OrderId,
    OrderSide Side,
    OrderType OrderType,
    decimal Size,
    decimal OutstandingSize,
    decimal ExecutedSize,
    decimal? Price,
    DateTimeOffset? OrderedAt = null,
    DateTimeOffset? UpdatedAt = null,
    string? ClientOrderId = null,
    decimal? StopPrice = null,
    string? Status = null);
