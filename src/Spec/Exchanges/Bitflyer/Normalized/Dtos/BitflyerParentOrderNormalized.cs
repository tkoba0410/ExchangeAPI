using System;
using System.Collections.Generic;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerParentOrderNormalized(
    long Id,
    string ParentOrderId,
    string ProductCode,
    BitflyerSide Side,
    BitflyerParentOrderType ParentOrderType,
    decimal Price,
    decimal AveragePrice,
    decimal Size,
    BitflyerParentOrderState ParentOrderState,
    DateTimeOffset ExpireDate,
    DateTimeOffset ParentOrderDate,
    string ParentOrderAcceptanceId,
    decimal OutstandingSize,
    decimal CancelSize,
    decimal ExecutedSize,
    decimal TotalCommission);

public sealed record BitflyerParentOrderDetailNormalized(
    long Id,
    string ParentOrderId,
    BitflyerOrderMethod OrderMethod,
    DateTimeOffset ExpireDate,
    BitflyerTimeInForce TimeInForce,
    IReadOnlyList<BitflyerParentOrderParameterNormalized> Parameters,
    string ParentOrderAcceptanceId);

public sealed record BitflyerParentOrderParameterNormalized(
    string ProductCode,
    BitflyerConditionType ConditionType,
    BitflyerSide Side,
    decimal Size,
    decimal Price,
    decimal TriggerPrice,
    decimal Offset);
