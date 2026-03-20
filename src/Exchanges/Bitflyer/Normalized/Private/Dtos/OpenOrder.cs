using System;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record OpenOrder(
    long Id,
    ProductCode ProductCode,
    Side Side,
    OrderType OrderType,
    Size Size,
    Price? AveragePrice,
    FreeText? Status,
    DateTimeOffset OrderedAt,
    DateTimeOffset ExpireDate,
    Size OutstandingSize,
    Size CancelSize,
    Size ExecutedSize,
    Price? Price,
    decimal TotalCommission,
    FreeText? TimeInForce = null,
    ExchangeOrderId? ExchangeOrderId = null,
    AcceptanceId? AcceptanceId = null);
