using System;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;

internal static class ExchangeInfoCallMapper
{
    public static Call<TReq, TOk> MapCall<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        Call<TNormReq, TNormRes> normalizedCall,
        string component,
        Func<TNormRes, TOk> mapper) =>
        AdapterCallMapper.MapCall(request, normalizedCall, component, mapper);

    public static Call<TReq, TOk> FromException<TReq, TOk>(
        TReq request,
        DateTimeOffset startedAt,
        string component,
        Exception ex) =>
        AdapterCallMapper.FromException<TReq, TOk>(request, startedAt, component, ex);
}
