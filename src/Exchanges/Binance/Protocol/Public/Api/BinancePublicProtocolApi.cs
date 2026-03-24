using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Protocol.Public.Api;

public sealed class BinancePublicProtocolApi : IBinancePublicProtocolApi
{
    private readonly IGetKlinesProtocolEndpoint _getKlines;

    public BinancePublicProtocolApi(IGetKlinesProtocolEndpoint getKlines)
    {
        _getKlines = getKlines;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetKlinesCallAsync(
        string symbol,
        string interval,
        long? startTime = null,
        long? endTime = null,
        string? timeZone = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        return _getKlines.SendAsync(symbol, interval, startTime, endTime, timeZone, limit, cancellationToken);
    }
}
