using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;

public sealed record BitflyerParentOrderNormalized(
    long Id,
    string ParentOrderId,
    string ProductCode,
    Closed<BitflyerSide> Side,
    Closed<BitflyerParentOrderType> ParentOrderType,
    Price? Price,
    Price? AveragePrice,
    Size Size,
    Closed<BitflyerParentOrderState> ParentOrderState,
    DateTimeOffset ExpireDate,
    DateTimeOffset ParentOrderDate,
    string ParentOrderAcceptanceId,
    Size OutstandingSize,
    Size CancelSize,
    Size ExecutedSize,
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
    Price? Price,
    Size? Size,
    Price? TriggerPrice,
    decimal? Offset);
