using ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;

public interface IGetKlinesProtocolEndpoint
{
    Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string symbol,
        string interval,
        long? startTime = null,
        long? endTime = null,
        string? timeZone = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}

public sealed class GetKlinesProtocolEndpoint : IGetKlinesProtocolEndpoint
{
    private const string Path = "/api/v3/klines";
    private readonly IProtocolTransport _transport;

    public GetKlinesProtocolEndpoint(IProtocolTransport transport)
    {
        _transport = transport;
    }

    public async Task<Call<ProtocolRequest, ProtocolResponse>> SendAsync(
        string symbol,
        string interval,
        long? startTime = null,
        long? endTime = null,
        string? timeZone = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ProtocolRequest
        {
            EndpointId = BinanceEndpointIds.GetKlines,
            Method = HttpMethods.Get,
            Path = Path,
            Query = ProtocolQueryFactory.Create(
                ("symbol", symbol),
                ("interval", interval),
                ("startTime", startTime?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                ("endTime", endTime?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
                ("timeZone", timeZone),
                ("limit", limit?.ToString(System.Globalization.CultureInfo.InvariantCulture))),
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
}
