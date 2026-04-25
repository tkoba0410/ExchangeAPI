using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Binance.Protocol.Public.Api;

public interface IBinancePublicProtocolApi
{
    Task<CallResult<ProtocolRequest, ProtocolResponse>> GetKlinesAsync(
        string symbol,
        string interval,
        long? startTime = null,
        long? endTime = null,
        string? timeZone = null,
        int? limit = null,
        CancellationToken cancellationToken = default);
}
