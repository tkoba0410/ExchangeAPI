using System;

namespace ExchangeApi.Spec.CallCommon;

public sealed record Call<TReq, TRes>(
    CallId Id,
    DateTimeOffset StartedAt,
    TimeSpan Duration,
    TReq Request,
    CallResult<TRes> Result,
    CallMeta Meta);
