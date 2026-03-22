using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Stage10.Bitflyer.Normalized.Internal.Errors;

internal static class NormalizedCallFactory
{
    public static Call<TRequest, TResponse> CreateSuccess<TRequest, TResponse>(
        TRequest request,
        string endpointId,
        string component,
        string scope,
        string auth,
        TResponse response,
        Call<WireCallSpec, WireResponse>? child = null) =>
        new(
            Id: CallId.New(),
            StartedAt: child?.StartedAt ?? DateTimeOffset.UtcNow,
            Duration: child?.Duration ?? TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Ok(response),
            Meta: CreateMeta(endpointId, component, scope, auth, stage: null, child));

    public static Call<TRequest, TResponse> CreateError<TRequest, TResponse>(
        TRequest request,
        string endpointId,
        string component,
        string scope,
        string auth,
        CallError error,
        string? stage = null,
        Call<WireCallSpec, WireResponse>? child = null) =>
        new(
            Id: CallId.New(),
            StartedAt: child?.StartedAt ?? DateTimeOffset.UtcNow,
            Duration: child?.Duration ?? TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TResponse>.Err(error),
            Meta: CreateMeta(endpointId, component, scope, auth, stage, child));

    private static CallMeta CreateMeta(
        string endpointId,
        string component,
        string scope,
        string auth,
        string? stage,
        Call<WireCallSpec, WireResponse>? child)
    {
        var tags = new Dictionary<string, string>
        {
            ["Scope"] = scope,
            ["Auth"] = auth,
            ["Retryable"] = "false",
        };

        if (!string.IsNullOrWhiteSpace(stage))
        {
            tags["Stage"] = stage;
        }

        return new CallMeta(
            Layer: "Normalized",
            Component: component,
            EndpointId: endpointId,
            Tags: tags,
            Children: child is null ? null : new[] { child.Id });
    }
}
