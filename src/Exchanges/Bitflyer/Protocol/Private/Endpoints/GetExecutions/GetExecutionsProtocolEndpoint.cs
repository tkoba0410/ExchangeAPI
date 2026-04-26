using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;

public interface IGetExecutionsProtocolEndpoint
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        CancellationToken cancellationToken = default);

    Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(productCode, count, before, after, childOrderId, childOrderAcceptanceId, cancellationToken);
    }
}

public sealed class GetExecutionsProtocolEndpoint : IGetExecutionsProtocolEndpoint
{
    private const string Path = "/v1/me/getexecutions";
    private readonly IProtocolTransport _transport;

    public GetExecutionsProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, childOrderId, childOrderAcceptanceId, null, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsync(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        IApiCredentialSession credentialSession,
        CancellationToken cancellationToken = default)
    {
        return SendAsyncCore(productCode, count, before, after, childOrderId, childOrderAcceptanceId, credentialSession, cancellationToken);
    }

    private async Task<CallResult<ProtocolRequest, ProtocolResponse>> SendAsyncCore(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId,
        IApiCredentialSession? credentialSession,
        CancellationToken cancellationToken)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetExecutionsPrivate,
            Method = HttpMethods.Get,
            Path = Path,
            Query = BuildQuery(productCode, count, before, after, childOrderId, childOrderAcceptanceId),
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

    private static IReadOnlyDictionary<string, string> BuildQuery(
        string productCode,
        int? count,
        long? before,
        long? after,
        string? childOrderId,
        string? childOrderAcceptanceId)
    {
        var query = new Dictionary<string, string>
        {
            ["product_code"] = productCode,
        };

        AddIfPresent(query, "count", count);
        AddIfPresent(query, "before", before);
        AddIfPresent(query, "after", after);
        AddIfPresent(query, "child_order_id", childOrderId);
        AddIfPresent(query, "child_order_acceptance_id", childOrderAcceptanceId);

        return query;
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

    private static void AddIfPresent(IDictionary<string, string> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query[key] = value;
        }
    }
}
