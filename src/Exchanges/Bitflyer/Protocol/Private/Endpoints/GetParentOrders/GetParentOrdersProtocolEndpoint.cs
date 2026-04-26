using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;

public interface IGetParentOrdersProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(productCode, count, before, after, parentOrderState, cancellationToken);
    }
}

public sealed class GetParentOrdersProtocolEndpoint : IGetParentOrdersProtocolEndpoint
{
    private const string Path = "/v1/me/getparentorders";
    private readonly IProtocolTransport _transport;

    public GetParentOrdersProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, parentOrderState, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, parentOrderState, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? parentOrderState,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetParentOrders,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("product_code", productCode),
                ("count", count?.ToString(CultureInfo.InvariantCulture)),
                ("before", before?.ToString(CultureInfo.InvariantCulture)),
                ("after", after?.ToString(CultureInfo.InvariantCulture)),
                ("parent_order_state", parentOrderState)),
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
