using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;

public interface IGetParentOrderProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default);
}

public sealed class GetParentOrderProtocolEndpoint : IGetParentOrderProtocolEndpoint
{
    private const string Path = "/v1/me/getparentorder";
    private readonly IProtocolTransport _transport;

    public GetParentOrderProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetParentOrder,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("parent_order_id", parentOrderId),
                ("parent_order_acceptance_id", parentOrderAcceptanceId)),
            BodyText = null,
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
