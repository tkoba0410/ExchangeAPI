using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Credentials;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;

public interface IGetCollateralHistoryProtocolEndpoint
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

public sealed class GetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
{
    private const string Path = "/v1/me/getcollateralhistory";
    private readonly IProtocolTransport _transport;

    public GetCollateralHistoryProtocolEndpoint(IProtocolTransport transport)
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
            EndpointId = BitflyerEndpointIds.GetCollateralHistory,
            Method = HttpMethods.Get,
            Path = Path,
            Query = BuildQuery(count, before, after),
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

    private static IReadOnlyDictionary<string, string>? BuildQuery(int? count, long? before, long? after)
    {
        var query = new Dictionary<string, string>();

        AddIfPresent(query, "count", count);
        AddIfPresent(query, "before", before);
        AddIfPresent(query, "after", after);

        return query.Count == 0 ? null : query;
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
