using System;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Adapter.Internal;

internal static class ExchangeInfoCallMapper
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
}
