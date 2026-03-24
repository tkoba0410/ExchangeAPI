using System.Globalization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;

public interface IGetBalanceHistoryProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? currencyCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default);
}

public sealed class GetBalanceHistoryProtocolEndpoint : IGetBalanceHistoryProtocolEndpoint
{
    private const string Path = "/v1/me/getbalancehistory";
    private readonly IProtocolTransport _transport;

    public GetBalanceHistoryProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string? currencyCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BitflyerEndpointIds.GetBalanceHistory,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("currency_code", currencyCode),
                ("count", count?.ToString(CultureInfo.InvariantCulture)),
                ("before", before?.ToString(CultureInfo.InvariantCulture)),
                ("after", after?.ToString(CultureInfo.InvariantCulture))),
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
}
