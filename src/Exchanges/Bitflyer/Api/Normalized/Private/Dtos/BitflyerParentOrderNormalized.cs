using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;

public sealed record BitflyerParentOrderNormalized(
    long Id,
    ExchangeOrderId ParentOrderId,
    ProductCode ProductCode,
    Closed<BitflyerSide> Side,
    Closed<BitflyerParentOrderType> ParentOrderType,
    Price? Price,
    Price? AveragePrice,
    Size Size,
    Closed<BitflyerParentOrderState> ParentOrderState,
    DateTimeOffset ExpireDate,
    DateTimeOffset ParentOrderDate,
    AcceptanceId ParentOrderAcceptanceId,
    Size OutstandingSize,
    Size CancelSize,
    Size ExecutedSize,
    decimal TotalCommission,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;

public sealed record BitflyerParentOrderDetailNormalized(
    long Id,
    ExchangeOrderId ParentOrderId,
    Closed<BitflyerOrderMethod> OrderMethod,
    DateTimeOffset ExpireDate,
    Closed<BitflyerTimeInForce> TimeInForce,
    IReadOnlyList<BitflyerParentOrderParameterNormalized> Parameters,
    AcceptanceId ParentOrderAcceptanceId,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;

public sealed record BitflyerParentOrderParameterNormalized(
    ProductCode ProductCode,
    Closed<BitflyerConditionType> ConditionType,
    Closed<BitflyerSide> Side,
    Price? Price,
    Size? Size,
    Price? TriggerPrice,
    decimal? Offset);
