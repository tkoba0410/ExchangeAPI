using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record OrdersResponse(
    IReadOnlyList<OrdersItem> Items,
    bool HasMore,
    Cursor? NextCursor,
    int RequestedLimit,
    int AppliedLimit,
    int ReturnedCount,
    bool LimitClamped,
    Completeness Completeness,
    PartialReason? PartialReason,
    DateTimeOffset AsOf);

public sealed record OrdersItem(
    DateTimeOffset CreatedAt,
    OrderId OrderId,
    Symbol Market,
    Side Side,
    OrdersOrderType OrderType,
    Price? Price,
    Size Size,
    OrdersOrderStatus Status);

public enum OrdersOrderType
{
    Limit,
    Market,
    Unknown
}

public enum OrdersOrderStatus
{
    Open,
    Filled,
    Canceled,
    Rejected,
    Unknown
}
