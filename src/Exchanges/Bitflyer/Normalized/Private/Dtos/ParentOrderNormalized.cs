using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Dtos;

public sealed record ParentOrderNormalized(
    long Id,
    ExchangeOrderId ParentOrderId,
    ProductCode ProductCode,
    Closed<ExchangeSide> Side,
    Closed<ParentOrderType> ParentOrderType,
    Price? Price,
    Price? AveragePrice,
    Size Size,
    Closed<ParentOrderState> ParentOrderState,
    DateTimeOffset ExpireDate,
    DateTimeOffset ParentOrderDate,
    AcceptanceId ParentOrderAcceptanceId,
    Size OutstandingSize,
    Size CancelSize,
    Size ExecutedSize,
    decimal TotalCommission,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;

public sealed record ParentOrderDetailNormalized(
    long Id,
    ExchangeOrderId ParentOrderId,
    Closed<OrderMethod> OrderMethod,
    DateTimeOffset ExpireDate,
    Closed<TimeInForce> TimeInForce,
    IReadOnlyList<ParentOrderParameterNormalized> Parameters,
    AcceptanceId ParentOrderAcceptanceId,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;

public sealed record ParentOrderParameterNormalized(
    ProductCode ProductCode,
    Closed<ConditionType> ConditionType,
    Closed<ExchangeSide> Side,
    Price? Price,
    Size? Size,
    Price? TriggerPrice,
    decimal? Offset);
