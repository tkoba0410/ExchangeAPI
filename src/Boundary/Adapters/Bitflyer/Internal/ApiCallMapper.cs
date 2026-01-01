using System;
using System.Net;
using ExchangeApi.Boundary.Adapters.Common.ApiCallMapping;
using ExchangeApi.Common.Enums;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;

internal static class ApiCallMapper
{
    public static ApiCallMeta ToMeta(CallMeta meta) =>
        ApiCallMapperBase.ToMeta(meta);

    public static ApiCallMeta ToMeta(DateTimeOffset startedAt, string? requestId = null) =>
        ApiCallMapperBase.ToMeta(startedAt, requestId);

    public static ApiCall<TReq, TOk, ApiError> Ok<TReq, TOk>(
        ExchangeCode exchange,
        TReq request,
        CallMeta meta,
        int statusCode,
        TOk value) =>
        ApiCallMapperBase.Ok(exchange, request, meta, statusCode, value);

    public static ApiCall<TReq, TOk, ApiError> Err<TReq, TOk>(
        ExchangeCode exchange,
        TReq request,
        CallMeta meta,
        int statusCode,
        string? message = null) =>
        ApiCallMapperBase.Err<TReq, TOk>(exchange, request, meta, statusCode, message);

    public static ApiCall<TReq, TOk, ApiError> Err<TReq, TOk>(
        ExchangeCode exchange,
        TReq request,
        ApiCallMeta meta,
        int statusCode,
        string? message = null) =>
        ApiCallMapperBase.Err<TReq, TOk>(exchange, request, meta, statusCode, message);

    public static ApiCall<TReq, TOk, ApiError> FromException<TReq, TOk>(
        ExchangeCode exchange,
        TReq request,
        DateTimeOffset startedAt,
        Exception ex) =>
        ApiCallMapperBase.FromException<TReq, TOk>(exchange, request, startedAt, ex);

    public static ExchangeErrorCategory? ToExchangeErrorCategory(ApiErrorKind kind) =>
        ApiCallMapperBase.ToExchangeErrorCategory(kind);

    public static HttpStatusCode? ToStatusCode(int statusCode) =>
        ApiCallMapperBase.ToStatusCode(statusCode);

    public static ApiErrorKind Classify(int statusCode, string? exchangeErrorCode, string? message) =>
        ApiCallMapperBase.Classify(statusCode, exchangeErrorCode, message);
}
