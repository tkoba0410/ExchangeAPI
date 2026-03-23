using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public interface IBitflyerPublicNativeApi
{
    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}
