using System;
using System.Net;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Adapter.Internal;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

internal static class ApiCallMapper
{
    public static Call<TReq, TOk> FromCall<TReq, TOk, TNormReq>(
        TReq request,
        Call<TNormReq, TOk> normalizedCall,
        string component) =>
        AdapterCallMapper.FromCall(request, normalizedCall, component);

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

    public static ExchangeErrorCategory? ToExchangeErrorCategory(CallError error) =>
        AdapterCallMapper.ToExchangeErrorCategory(error);

    public static HttpStatusCode? ToStatusCode(int? statusCode) =>
        AdapterCallMapper.ToStatusCode(statusCode);
}
