using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;

public interface ISendParentOrderProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);
}

public sealed class SendParentOrderProtocolEndpoint : ISendParentOrderProtocolEndpoint
{
    private const string Path = "/v1/me/sendparentorder";
    private readonly IProtocolTransport _transport;

    public SendParentOrderProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.SendParentOrder,
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
