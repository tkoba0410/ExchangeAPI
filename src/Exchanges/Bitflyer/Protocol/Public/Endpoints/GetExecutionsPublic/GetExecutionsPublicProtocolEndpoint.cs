using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;

public interface IGetExecutionsPublicProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);
}

public sealed class GetExecutionsPublicProtocolEndpoint : IGetExecutionsPublicProtocolEndpoint
{
    private const string Path = "/v1/getexecutions";
    private readonly IProtocolTransport _transport;

    public GetExecutionsPublicProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetExecutionsPublic,
            Method = HttpMethods.Get,
            Path = Path,
            Query = BuildQuery(productCode, count, before, after),
            BodyText = null,
        };

        var result = await _transport.SendAsync(request, ProtocolTransportAuthMode.None, cancellationToken);
        return ProtocolCallFactory.ToProtocolCall(
            request,
            result,
            scope: "Public",
            auth: "None",
            component: CallComponents.PublicEndpointModule);
    }

    private static IReadOnlyDictionary<string, string>? BuildQuery(
        string? productCode,
        int? count,
        long? before,
        long? after)
    {
        Dictionary<string, string>? query = null;

        AddIfPresent(ref query, "product_code", productCode);
        AddIfPresent(ref query, "count", count);
        AddIfPresent(ref query, "before", before);
        AddIfPresent(ref query, "after", after);

        return query;
    }

    private static void AddIfPresent(ref Dictionary<string, string>? query, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        query ??= [];
        query[key] = value;
    }

    private static void AddIfPresent(ref Dictionary<string, string>? query, string key, int? value)
    {
        if (value is null)
        {
            return;
        }

        query ??= [];
        query[key] = value.Value.ToString(CultureInfo.InvariantCulture);
    }

    private static void AddIfPresent(ref Dictionary<string, string>? query, string key, long? value)
    {
        if (value is null)
        {
            return;
        }

        query ??= [];
        query[key] = value.Value.ToString(CultureInfo.InvariantCulture);
    }
}
