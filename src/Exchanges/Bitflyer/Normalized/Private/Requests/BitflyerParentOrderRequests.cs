using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record SendParentOrderRequest(
    IReadOnlyList<BitflyerParentOrderParameterRequest> Parameters,
    BitflyerOrderMethod? OrderMethod = null,
    int? MinuteToExpire = null,
    BitflyerTimeInForce? TimeInForce = null);

public sealed record BitflyerParentOrderParameterRequest(
    string ProductCode,
    BitflyerConditionType ConditionType,
    BitflyerSide Side,
    Size Size,
    Price? Price = null,
    Price? TriggerPrice = null,
    decimal? Offset = null);

public sealed record CancelParentOrderRequest(
    string ProductCode,
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null);

public sealed record GetParentOrdersRequest(
    string ProductCode,
    BitflyerParentOrderState? ParentOrderState = null,
    int? Count = null,
    long? Before = null,
    long? After = null);

public sealed record GetParentOrderRequest(
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null);
