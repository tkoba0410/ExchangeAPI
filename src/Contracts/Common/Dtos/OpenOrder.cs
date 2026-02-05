using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Contracts.Common.Dtos;

/// <summary>
/// オープンな子注文の概要（受付IDなし、Exchange/時刻を含む）。
/// </summary>
public sealed record OpenOrder(
    Symbol Symbol,
    OrderKey Key,
    Side Side,
    OrderType OrderType,
    Size Size,
    Size OutstandingSize,
    Size ExecutedSize,
    Price? Price,
    DateTimeOffset? OrderedAt = null,
    DateTimeOffset? UpdatedAt = null,
    Price? StopPrice = null,
    FreeText? Status = null,
    ExchangeOrderId? ExchangeOrderId = null,
    AcceptanceId? AcceptanceId = null);
