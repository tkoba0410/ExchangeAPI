using System;
using System.Net;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Adapter.Internal;

internal static class ApiCallMapperBase
{
    public static Call<TReq, TOk> MapCall<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        Call<TNormReq, TNormRes> normalizedCall,
        string component,
        Func<TNormRes, TOk> mapper)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: component,
            EndpointId: normalizedCall.Meta.EndpointId,
            Tags: null,
            Children: new[] { normalizedCall.Id });

        return normalizedCall.Result switch
        {
            CallResult<TNormRes>.Err err => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: normalizedCall.StartedAt,
                Duration: normalizedCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(err.Error),
                Meta: meta),
            CallResult<TNormRes>.Ok ok => MapOk(request, normalizedCall, component, ok.Response, mapper, meta),
            _ => new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: normalizedCall.StartedAt,
                Duration: normalizedCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, "Normalized call returned unknown result.")),
                Meta: meta)
        };
    }

    public static Call<TReq, TOk> FromCall<TReq, TOk, TNormReq>(
        TReq request,
        Call<TNormReq, TOk> normalizedCall,
        string component)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: component,
            EndpointId: normalizedCall.Meta.EndpointId,
            Tags: null,
            Children: new[] { normalizedCall.Id });

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: normalizedCall.StartedAt,
            Duration: normalizedCall.Duration,
            Request: request,
            Result: normalizedCall.Result,
            Meta: meta);
    }

    public static Call<TReq, TOk> FromException<TReq, TOk>(
        TReq request,
        DateTimeOffset startedAt,
        string component,
        Exception ex)
    {
        var meta = new CallMeta(
            Layer: "Contracts",
            Component: component,
            EndpointId: CallMeta.InternalEndpointId,
            Tags: null,
            Children: null);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: startedAt,
            Duration: DateTimeOffset.UtcNow - startedAt,
            Request: request,
            Result: new CallResult<TOk>.Err(new CallError(CallErrorKind.Unknown, ex.Message, ex)),
            Meta: meta);
    }

    private static Call<TReq, TOk> MapOk<TReq, TNormReq, TNormRes, TOk>(
        TReq request,
        Call<TNormReq, TNormRes> normalizedCall,
        string component,
        TNormRes response,
        Func<TNormRes, TOk> mapper,
        CallMeta meta)
    {
        try
        {
            var mapped = mapper(response);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: normalizedCall.StartedAt,
                Duration: normalizedCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Ok(mapped),
                Meta: meta);
        }
        catch (Exception ex)
        {
            var error = new CallError(CallErrorKind.Mapping, $"{component} failed to map response.", ex);
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: normalizedCall.StartedAt,
                Duration: normalizedCall.Duration,
                Request: request,
                Result: new CallResult<TOk>.Err(error),
                Meta: meta);
        }
    }

    public static ExchangeErrorCategory? ToExchangeErrorCategory(CallError error)
    {
        if (error is null) return ExchangeErrorCategory.Unknown;

        return error.Kind switch
        {
            CallErrorKind.Transport => ExchangeErrorCategory.Network,
            CallErrorKind.Codec => ExchangeErrorCategory.Server,
            CallErrorKind.Mapping => ExchangeErrorCategory.Request,
            CallErrorKind.Semantic => ExchangeErrorCategory.Request,
            CallErrorKind.Http => MapHttpErrorCategory(error),
            _ => ExchangeErrorCategory.Unknown,
        };
    }

    private static ExchangeErrorCategory MapHttpErrorCategory(CallError error)
    {
        if (error.HttpStatus is int status)
        {
            return status switch
            {
                400 or 422 => ExchangeErrorCategory.Request,
                401 or 403 => ExchangeErrorCategory.Auth,
                404 => ExchangeErrorCategory.Request,
                429 => ExchangeErrorCategory.RateLimit,
                >= 500 => ExchangeErrorCategory.Server,
                _ => ExchangeErrorCategory.Unknown,
            };
        }

        if (error.Exception is ExchangeApiException { ExchangeErrorCode: not null })
        {
            return ExchangeErrorCategory.Request;
        }

        return ExchangeErrorCategory.Unknown;
    }

    public static HttpStatusCode? ToStatusCode(int? statusCode) =>
        statusCode is > 0 ? (HttpStatusCode?)statusCode : null;
}
