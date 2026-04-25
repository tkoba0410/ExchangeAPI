using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Native.Internal.Shared;

internal static class NativeCallFactory
{
    internal static CallResult<TRequest, TResponse> Success<TRequest, TResponse>(
        TRequest request,
        TResponse response,
        CallResult<ProtocolRequest, ProtocolResponse> protocolCall)
    {
        return CallFactory.Success(
            request,
            response,
            new CallMeta
            {
                Layer = CallLayers.Native,
                Component = CallComponents.PublicEndpointModule,
                EndpointId = protocolCall.Request.EndpointId,
                Scope = "Public",
                Auth = "None",
                Children = [protocolCall],
            });
    }

    internal static CallResult<TRequest, TResponse> Failure<TRequest, TResponse>(
        TRequest request,
        CallError error,
        CallResult<ProtocolRequest, ProtocolResponse>? protocolCall,
        string endpointId)
    {
        return CallFactory.Failure<TRequest, TResponse>(
            request,
            error,
            new CallMeta
            {
                Layer = CallLayers.Native,
                Component = CallComponents.PublicEndpointModule,
                EndpointId = endpointId,
                Scope = "Public",
                Auth = "None",
                Children = protocolCall is null ? null : [protocolCall],
            });
    }
}
