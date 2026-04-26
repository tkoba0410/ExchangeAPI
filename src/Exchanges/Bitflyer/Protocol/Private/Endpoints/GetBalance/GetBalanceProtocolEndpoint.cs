using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;

public interface IGetBalanceProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(cancellationToken);
    }
}

public sealed class GetBalanceProtocolEndpoint : IGetBalanceProtocolEndpoint
{
    private const string Path = "/v1/me/getbalance";
    private readonly IProtocolTransport _transport;

    public GetBalanceProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetBalance,
            Method = HttpMethods.Get,
            Path = Path,
            Query = null,
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
