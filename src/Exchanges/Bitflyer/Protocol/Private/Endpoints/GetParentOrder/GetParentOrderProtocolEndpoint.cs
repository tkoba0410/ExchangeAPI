using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;

public interface IGetParentOrderProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(parentOrderId, parentOrderAcceptanceId, cancellationToken);
    }
}

public sealed class GetParentOrderProtocolEndpoint : IGetParentOrderProtocolEndpoint
{
    private const string Path = "/v1/me/getparentorder";
    private readonly IProtocolTransport _transport;

    public GetParentOrderProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(parentOrderId, parentOrderAcceptanceId, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(parentOrderId, parentOrderAcceptanceId, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        string? parentOrderId,
        string? parentOrderAcceptanceId,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
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

        var result = await (credentialSession is null
            ? _transport.SendAsync(request, ProtocolTransportAuthMode.KeySecret, cancellationToken)
            : _transport.SendAsync(request, ProtocolTransportAuthMode.KeySecret, credentialSession, cancellationToken));
        return ProtocolCallFactory.ToProtocolCall(
            request,
            result,
            scope: "Private",
            auth: "KeySecret",
            component: CallComponents.PrivateEndpointModule);
    }
}
