using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public interface IBitflyerPublicNativeApi
{
    Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetHealthRequest, GetHealthResponse>> GetHealthCallAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateCallAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetChatsRequest, IReadOnlyList<GetChats.Item>>> GetChatsCallAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> GetExecutionsCallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}
