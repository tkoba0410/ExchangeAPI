using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record SendParentOrderRequest(
    IReadOnlyList<ParentOrderParameterRequest> Parameters,
    OrderMethod? OrderMethod = null,
    MinuteToExpire? MinuteToExpire = null,
    TimeInForce? TimeInForce = null);

public sealed record ParentOrderParameterRequest(
    ProductCode ProductCode,
    ConditionType ConditionType,
    ExchangeSide Side,
    Size Size,
    Price? Price = null,
    Price? TriggerPrice = null,
    PriceOffset? Offset = null);

public sealed record CancelParentOrderRequest(
    ProductCode ProductCode,
    ExchangeOrderId? ParentOrderId = null,
    AcceptanceId? ParentOrderAcceptanceId = null);

public sealed record GetParentOrdersRequest(
    ProductCode ProductCode,
    ParentOrderState? ParentOrderState = null,
    RequestCount? Count = null,
    RequestBefore? Before = null,
    RequestAfter? After = null);

public sealed record GetParentOrderRequest(
    ExchangeOrderId? ParentOrderId = null,
    AcceptanceId? ParentOrderAcceptanceId = null);
