using System;

namespace ExchangeApi.Spec.CallCommon;

public sealed record CallMeta(
    DateTimeOffset StartedAt,
    TimeSpan Elapsed,
    string? RequestId);
