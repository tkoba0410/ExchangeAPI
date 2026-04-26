using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;

public interface IGetChildOrdersProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId, cancellationToken);
    }
}

public sealed class GetChildOrdersProtocolEndpoint : IGetChildOrdersProtocolEndpoint
{
    private const string Path = "/v1/me/getchildorders";
    private readonly IProtocolTransport _transport;

    public GetChildOrdersProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, childOrderState, childOrderId, childOrderAcceptanceId, parentOrderId, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetChildOrders,
            Method = HttpMethods.Get,
            Path = Path,
            Query = BuildQuery(
                productCode,
                count,
                before,
                after,
                childOrderState,
                childOrderId,
                childOrderAcceptanceId,
                parentOrderId),
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

    private static IReadOnlyDictionary<string, string>? BuildQuery(
        string? productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderState,
        string? childOrderId,
        string? childOrderAcceptanceId,
        string? parentOrderId)
    {
        var query = new Dictionary<string, string>();

        AddIfPresent(query, "product_code", productCode);
        AddIfPresent(query, "count", count);
        AddIfPresent(query, "before", before);
        AddIfPresent(query, "after", after);
        AddIfPresent(query, "child_order_state", childOrderState);
        AddIfPresent(query, "child_order_id", childOrderId);
        AddIfPresent(query, "child_order_acceptance_id", childOrderAcceptanceId);
        AddIfPresent(query, "parent_order_id", parentOrderId);

        return query.Count == 0 ? null : query;
    }

    private static void AddIfPresent(IDictionary<string, string> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query[key] = value;
        }
    }

    private static void AddIfPresent(IDictionary<string, string> query, string key, int? value)
    {
        if (value is not null)
        {
            query[key] = value.Value.ToString(CultureInfo.InvariantCulture);
        }
    }

    private static void AddIfPresent(IDictionary<string, string> query, string key, long? value)
    {
        if (value is not null)
        {
            query[key] = value.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
