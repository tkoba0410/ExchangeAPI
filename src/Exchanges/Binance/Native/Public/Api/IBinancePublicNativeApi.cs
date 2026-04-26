using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Api;

public interface IBinancePublicNativeApi
{
    Task<CallResult<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default);
}
