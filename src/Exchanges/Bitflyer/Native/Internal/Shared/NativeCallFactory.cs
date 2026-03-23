using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Internal.Shared;

internal static class NativeCallFactory
{
    internal static Call<TRequest, TResponse> Success<TRequest, TResponse>(
        TRequest request,
        TResponse response,
        Call<ProtocolRequest, ProtocolResponse> protocolCall,
        string scope)
    {
        return CallFactory.Success(
            request,
            response,
            CreateMeta(protocolCall.Request.EndpointId, scope, protocolCall.Request, protocolCall));
    }

    internal static Call<TRequest, TResponse> Failure<TRequest, TResponse>(
        TRequest request,
        CallError error,
        Call<ProtocolRequest, ProtocolResponse>? protocolCall,
        string endpointId,
        string scope,
        string auth)
    {
        return CallFactory.Failure<TRequest, TResponse>(
            request,
            error,
            new CallMeta
            {
                Layer = CallLayers.Native,
                Component = scope == "Public" ? CallComponents.PublicEndpointModule : CallComponents.PrivateEndpointModule,
                EndpointId = endpointId,
                Scope = scope,
                Auth = auth,
                Children = protocolCall is null ? null : [protocolCall],
            });
    }

    private static CallMeta CreateMeta(
        string endpointId,
        string scope,
        ProtocolRequest protocolRequest,
        Call<ProtocolRequest, ProtocolResponse> protocolCall)
    {
        return new CallMeta
        {
            Layer = CallLayers.Native,
            Component = scope == "Public" ? CallComponents.PublicEndpointModule : CallComponents.PrivateEndpointModule,
            EndpointId = endpointId,
            Scope = scope,
            Auth = protocolRequest.Path.StartsWith("/v1/me/", StringComparison.Ordinal) ? "KeySecret" : "None",
            Children = [protocolCall],
        };
    }
}
