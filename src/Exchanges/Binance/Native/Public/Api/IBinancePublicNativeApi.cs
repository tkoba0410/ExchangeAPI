using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Binance.Native.Public.Api;

public interface IBinancePublicNativeApi
{
    Task<Call<GetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default);
}
