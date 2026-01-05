using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Types;
using ExchangeApi.Spec.ValueCommon.ClosedSet;
using ExchangeApi.Spec.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerParentOrderNormalized(
    long Id,
    string ParentOrderId,
    string ProductCode,
    Closed<BitflyerSide> Side,
    Closed<BitflyerParentOrderType> ParentOrderType,
    decimal Price,
    decimal AveragePrice,
    decimal Size,
    Closed<BitflyerParentOrderState> ParentOrderState,
    DateTimeOffset ExpireDate,
    DateTimeOffset ParentOrderDate,
    string ParentOrderAcceptanceId,
    decimal OutstandingSize,
    decimal CancelSize,
    decimal ExecutedSize,
    decimal TotalCommission,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;

public sealed record BitflyerParentOrderDetailNormalized(
    long Id,
    string ParentOrderId,
    Closed<BitflyerOrderMethod> OrderMethod,
    DateTimeOffset ExpireDate,
    Closed<BitflyerTimeInForce> TimeInForce,
    IReadOnlyList<BitflyerParentOrderParameterNormalized> Parameters,
    string ParentOrderAcceptanceId,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;

public sealed record BitflyerParentOrderParameterNormalized(
    string ProductCode,
    Closed<BitflyerConditionType> ConditionType,
    Closed<BitflyerSide> Side,
    decimal Size,
    decimal Price,
    decimal TriggerPrice,
    decimal Offset);
