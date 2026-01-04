using System;
using System.Net;
using ExchangeApi.Boundary.Adapters.Common.ApiCallMapping;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

internal static class ApiCallMapper
{
    public static Call<TReq, TOk> FromCall<TReq, TOk, TNormReq>(
        TReq request,
        Call<TNormReq, TOk> normalizedCall,
        string component) =>
        ApiCallMapperBase.FromCall(request, normalizedCall, component);

    public static Call<TReq, TOk> MapCall<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        Call<TNormReq, TNormRes> normalizedCall,
        string component,
        Func<TNormRes, TOk> mapper) =>
        ApiCallMapperBase.MapCall(request, normalizedCall, component, mapper);

    public static Call<TReq, TOk> FromException<TReq, TOk>(
        TReq request,
        DateTimeOffset startedAt,
        string component,
        Exception ex) =>
        ApiCallMapperBase.FromException<TReq, TOk>(request, startedAt, component, ex);

    public static ExchangeErrorCategory? ToExchangeErrorCategory(CallError error) =>
        ApiCallMapperBase.ToExchangeErrorCategory(error);

    public static HttpStatusCode? ToStatusCode(int? statusCode) =>
        ApiCallMapperBase.ToStatusCode(statusCode);
}
