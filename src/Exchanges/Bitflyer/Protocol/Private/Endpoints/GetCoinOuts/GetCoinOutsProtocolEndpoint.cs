using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;

public interface IGetCoinOutsProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(count, before, after, cancellationToken);
    }
}

public sealed class GetCoinOutsProtocolEndpoint : IGetCoinOutsProtocolEndpoint
{
    private const string Path = "/v1/me/getcoinouts";
    private readonly IProtocolTransport _transport;

    public GetCoinOutsProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(count, before, after, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(count, before, after, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        int? count,
        long? before,
        long? after,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetCoinOuts,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("count", count?.ToString(CultureInfo.InvariantCulture)),
                ("before", before?.ToString(CultureInfo.InvariantCulture)),
                ("after", after?.ToString(CultureInfo.InvariantCulture))),
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
