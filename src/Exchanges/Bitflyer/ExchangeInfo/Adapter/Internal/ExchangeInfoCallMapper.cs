using System;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Internal;

internal static class ExchangeInfoCallMapper
{
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
}
