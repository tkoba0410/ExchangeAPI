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

public sealed class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IGetMarketsNativeEndpoint _getMarkets;
    private readonly IGetBoardNativeEndpoint _getBoard;
    private readonly IGetBoardStateNativeEndpoint _getBoardState;
    private readonly IGetHealthNativeEndpoint _getHealth;
    private readonly IGetFundingRateNativeEndpoint _getFundingRate;
    private readonly IGetCorporateLeverageNativeEndpoint _getCorporateLeverage;
    private readonly IGetChatsNativeEndpoint _getChats;
    private readonly IGetExecutionsPublicNativeEndpoint _getExecutions;
    private readonly IGetTickerNativeEndpoint _getTicker;

    public BitflyerPublicNativeApi(
        IGetMarketsNativeEndpoint getMarkets,
        IGetBoardNativeEndpoint getBoard,
        IGetBoardStateNativeEndpoint getBoardState,
        IGetHealthNativeEndpoint getHealth,
        IGetFundingRateNativeEndpoint getFundingRate,
        IGetCorporateLeverageNativeEndpoint getCorporateLeverage,
        IGetChatsNativeEndpoint getChats,
        IGetExecutionsPublicNativeEndpoint getExecutions,
        IGetTickerNativeEndpoint getTicker)
    {
        _getMarkets = getMarkets;
        _getBoard = getBoard;
        _getBoardState = getBoardState;
        _getHealth = getHealth;
        _getFundingRate = getFundingRate;
        _getCorporateLeverage = getCorporateLeverage;
        _getChats = getChats;
        _getExecutions = getExecutions;
        _getTicker = getTicker;
    }

    public Task<CallResult<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getMarkets.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetBoardRequest, GetBoardResponse>> GetBoardAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
        GetBoardStateRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBoardState.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetHealthRequest, GetHealthResponse>> GetHealthAsync(
        GetHealthRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getHealth.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetFundingRateRequest, GetFundingRateResponse>> GetFundingRateAsync(
        GetFundingRateRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getFundingRate.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageAsync(
        GetCorporateLeverageRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getCorporateLeverage.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetChatsRequest, IReadOnlyList<GetChats.Item>>> GetChatsAsync(
        GetChatsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getChats.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> GetExecutionsAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getExecutions.CallAsync(request, cancellationToken);
    }

    public Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.CallAsync(request, cancellationToken);
    }
}
