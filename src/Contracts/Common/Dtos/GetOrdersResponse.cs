using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record GetOrdersResponse(
    IReadOnlyList<GetOrdersItem> Items,
    bool HasMore,
    Cursor? NextCursor,
    int RequestedLimit,
    int AppliedLimit,
    int ReturnedCount,
    bool LimitClamped,
    Completeness Completeness,
    PartialReason? PartialReason,
    DateTimeOffset AsOf);

public sealed record GetOrdersItem(
    DateTimeOffset CreatedAt,
    OrderId OrderId,
    Symbol Market,
    Side Side,
    GetOrdersOrderType OrderType,
    Price? Price,
    Size Size,
    GetOrdersOrderStatus Status);

public enum GetOrdersOrderType
{
    Limit,
    Market,
    Unknown
}

public enum GetOrdersOrderStatus
{
    Open,
    Filled,
    Canceled,
    Rejected,
    Unknown
}
