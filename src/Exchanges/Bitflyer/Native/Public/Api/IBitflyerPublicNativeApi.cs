using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public interface IBitflyerPublicNativeApi
{
    Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}
