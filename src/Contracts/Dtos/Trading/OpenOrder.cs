using System;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
namespace ExchangeApi.Contracts.Dtos.Trading;

/// <summary>
/// オープンな子注文の概要（受付IDなし、Exchange/時刻を含む）。
/// </summary>
public sealed record OpenOrder(
    ExchangeCode ExchangeCode,
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
    string? Status = null,
    string? ExchangeOrderId = null,
    string? AcceptanceId = null);
