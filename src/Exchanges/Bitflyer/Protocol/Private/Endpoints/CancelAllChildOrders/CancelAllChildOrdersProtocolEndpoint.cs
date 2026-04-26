using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;

public interface ICancelAllChildOrdersProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(bodyJson, cancellationToken);
    }
}

public sealed class CancelAllChildOrdersProtocolEndpoint : ICancelAllChildOrdersProtocolEndpoint
{
    private const string Path = "/v1/me/cancelallchildorders";
    private readonly IProtocolTransport _transport;

    public CancelAllChildOrdersProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(bodyJson, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string bodyJson,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(bodyJson, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        string bodyJson,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.CancelAllChildOrders,
            Method = HttpMethods.Post,
            Path = Path,
            Query = null,
            BodyText = bodyJson,
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
