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
    Task<CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBoardRequest, GetBoardResponse>> GetBoardAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetHealthRequest, GetHealthResponse>> GetHealthAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>>> GetChatsAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> GetExecutionsAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default);
}
