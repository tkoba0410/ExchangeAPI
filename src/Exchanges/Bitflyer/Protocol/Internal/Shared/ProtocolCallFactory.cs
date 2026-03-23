using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;

internal static class ProtocolCallFactory
{
    internal static Call<ProtocolRequest, ProtocolResponse> ToProtocolCall(
        ProtocolRequest request,
        ProtocolTransportResult result,
        string scope,
        string auth,
        string component)
    {
        var meta = new CallMeta
        {
            Layer = CallLayers.Protocol,
            Component = component,
            EndpointId = request.EndpointId,
            Scope = scope,
            Auth = auth,
            Children = null,
        };

        if (result.IsSuccess)
        {
            return CallFactory.Success(request, result.Response!, meta);
        }

        return CallFactory.Failure<ProtocolRequest, ProtocolResponse>(request, result.Error!, meta);
    }
}
