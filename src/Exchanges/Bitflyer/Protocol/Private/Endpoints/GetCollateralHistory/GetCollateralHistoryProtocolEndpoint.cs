using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;

public interface IGetCollateralHistoryProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);
}

public sealed class GetCollateralHistoryProtocolEndpoint : IGetCollateralHistoryProtocolEndpoint
{
    private const string Path = "/v1/me/getcollateralhistory";
    private readonly IProtocolTransport _transport;

    public GetCollateralHistoryProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetCollateralHistory,
            Method = HttpMethods.Get,
            Path = Path,
            Query = BuildQuery(count, before, after),
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
