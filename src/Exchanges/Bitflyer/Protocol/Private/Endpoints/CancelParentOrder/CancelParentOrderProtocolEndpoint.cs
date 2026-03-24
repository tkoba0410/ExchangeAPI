using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;

public interface ICancelParentOrderProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}

public sealed class CancelParentOrderProtocolEndpoint : ICancelParentOrderProtocolEndpoint
{
    private const string Path = "/v1/me/cancelparentorder";
    private readonly IProtocolTransport _transport;

    public CancelParentOrderProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.CancelParentOrder,
            Method = HttpMethods.Post,
            Path = Path,
            Query = null,
            BodyText = bodyJson,
        };

        var result = await _transport.SendAsync(request, ProtocolTransportAuthMode.KeySecret, cancellationToken);
        return ProtocolCallFactory.ToProtocolCall(
            request,
            result,
            scope: "Private",
            auth: "KeySecret",
            component: CallComponents.PrivateEndpointModule);
    }
}
