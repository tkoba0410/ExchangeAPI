using System;

namespace ExchangeApi.Contracts.Dtos.Common;

public sealed record PageMeta(
    int RequestedLimit,
    int AppliedLimit,
    int ReturnedCount,
    bool LimitClamped,
    Completeness Completeness,
    PartialReason? PartialReason,
    DateTimeOffset AsOf);
