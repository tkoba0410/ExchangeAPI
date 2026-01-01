using System;
using System.Net;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal;

internal static class ApiCallMapper
{
    public static ApiCallMeta ToMeta(CallMeta meta) =>
        new(meta.StartedAt, meta.Elapsed, meta.RequestId);

    public static ApiCallMeta ToMeta(DateTimeOffset startedAt, string? requestId = null) =>
        new(startedAt, DateTimeOffset.UtcNow - startedAt, requestId);

    public static ApiCall<TReq, TOk, ApiError> Ok<TReq, TOk>(
        ExchangeApi.Common.Enums.ExchangeCode exchange,
        TReq request,
        CallMeta meta,
        int statusCode,
        TOk value) =>
        new(exchange, request, ToMeta(meta), new ApiOk<TOk, ApiError>(value, statusCode));

    public static ApiCall<TReq, TOk, ApiError> Err<TReq, TOk>(
        ExchangeApi.Common.Enums.ExchangeCode exchange,
        TReq request,
        CallMeta meta,
        int statusCode,
        string? message = null) =>
        new(exchange, request, ToMeta(meta), new ApiErr<TOk, ApiError>(ToError(statusCode, meta.RequestId, message), statusCode));

    public static ApiCall<TReq, TOk, ApiError> Err<TReq, TOk>(
        ExchangeApi.Common.Enums.ExchangeCode exchange,
        TReq request,
        ApiCallMeta meta,
        int statusCode,
        string? message = null) =>
        new(exchange, request, meta, new ApiErr<TOk, ApiError>(ToError(statusCode, meta.RequestId, message), statusCode));

    public static ApiCall<TReq, TOk, ApiError> FromException<TReq, TOk>(
        ExchangeApi.Common.Enums.ExchangeCode exchange,
        TReq request,
        DateTimeOffset startedAt,
        Exception ex) =>
        new(exchange, request, ToMeta(startedAt), new ApiErr<TOk, ApiError>(ToError(0, null, ex.Message), 0));

    public static ExchangeErrorCategory? ToExchangeErrorCategory(ApiErrorKind kind) =>
        kind switch
        {
            ApiErrorKind.Validation => ExchangeErrorCategory.Request,
            ApiErrorKind.Auth => ExchangeErrorCategory.Auth,
            ApiErrorKind.RateLimit => ExchangeErrorCategory.RateLimit,
            ApiErrorKind.Timeout => ExchangeErrorCategory.Network,
            ApiErrorKind.HttpError => ExchangeErrorCategory.Server,
            _ => ExchangeErrorCategory.Unknown,
        };

    public static HttpStatusCode? ToStatusCode(int statusCode) =>
        statusCode > 0 ? (HttpStatusCode)statusCode : null;

    private static ApiError ToError(int statusCode, string? requestId, string? message)
    {
        var kind = Classify(statusCode, exchangeErrorCode: null, message);
        var resolved = string.IsNullOrWhiteSpace(message) ? DefaultMessage(kind) : message!;
        return new ApiError(kind, resolved, statusCode, requestId);
    }

    public static ApiErrorKind Classify(int statusCode, string? exchangeErrorCode, string? message) =>
        statusCode switch
        {
            400 or 422 => ApiErrorKind.Validation,
            401 or 403 => ApiErrorKind.Auth,
            404 => ApiErrorKind.NotFound,
            408 or 504 => ApiErrorKind.Timeout,
            429 => ApiErrorKind.RateLimit,
            >= 500 and <= 599 => ApiErrorKind.HttpError,
            _ => ApiErrorKind.Unknown,
        };

    private static string DefaultMessage(ApiErrorKind kind) =>
        kind switch
        {
            ApiErrorKind.Auth => "Authentication failed.",
            ApiErrorKind.NotFound => "Resource not found.",
            ApiErrorKind.Timeout => "Request timed out.",
            ApiErrorKind.RateLimit => "Rate limit exceeded.",
            ApiErrorKind.Validation => "Request validation failed.",
            ApiErrorKind.Canceled => "Request was canceled.",
            ApiErrorKind.HttpError => "HTTP error.",
            _ => "Unknown error.",
        };
}
