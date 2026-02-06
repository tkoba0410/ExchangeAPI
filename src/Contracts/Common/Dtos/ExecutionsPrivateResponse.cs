using System;
using System.Collections.Generic;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Common.Dtos;

public sealed record ExecutionsPrivateResponse(
    IReadOnlyList<ExecutionsPrivateItem> Items,
    bool HasMore,
    Cursor? NextCursor,
    int RequestedLimit,
    int AppliedLimit,
    int ReturnedCount,
    bool LimitClamped,
    Completeness Completeness,
    PartialReason? PartialReason,
    DateTimeOffset AsOf);

public sealed record ExecutionsPrivateItem(
    DateTimeOffset Timestamp,
    ExecutionId ExecutionId,
    Symbol Market,
    Side Side,
    Price Price,
    Size Size);
