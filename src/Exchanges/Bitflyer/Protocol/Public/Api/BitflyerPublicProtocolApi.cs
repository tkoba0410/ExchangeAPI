using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetHealth;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public sealed class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IGetMarketsProtocolEndpoint _getMarkets;
    private readonly IGetBoardProtocolEndpoint _getBoard;
    private readonly IGetBoardStateProtocolEndpoint _getBoardState;
    private readonly IGetHealthProtocolEndpoint _getHealth;
    private readonly IGetFundingRateProtocolEndpoint _getFundingRate;
    private readonly IGetCorporateLeverageProtocolEndpoint _getCorporateLeverage;
    private readonly IGetChatsProtocolEndpoint _getChats;
    private readonly IGetExecutionsPublicProtocolEndpoint _getExecutions;
    private readonly IGetTickerProtocolEndpoint _getTicker;

    public BitflyerPublicProtocolApi(
        IGetMarketsProtocolEndpoint getMarkets,
        IGetBoardProtocolEndpoint getBoard,
        IGetBoardStateProtocolEndpoint getBoardState,
        IGetHealthProtocolEndpoint getHealth,
        IGetFundingRateProtocolEndpoint getFundingRate,
        IGetCorporateLeverageProtocolEndpoint getCorporateLeverage,
        IGetChatsProtocolEndpoint getChats,
        IGetExecutionsPublicProtocolEndpoint getExecutions,
        IGetTickerProtocolEndpoint getTicker)
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

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetMarketsAsync(
        CancellationToken cancellationToken = default)
    {
        return _getMarkets.SendAsync(cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBoardAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.SendAsync(productCode, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetBoardStateAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getBoardState.SendAsync(productCode, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetHealthAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getHealth.SendAsync(productCode, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetFundingRateAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        return _getFundingRate.SendAsync(productCode, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetCorporateLeverageAsync(
        CancellationToken cancellationToken = default)
    {
        return _getCorporateLeverage.SendAsync(cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetChatsAsync(
        string? fromDate,
        CancellationToken cancellationToken = default)
    {
        return _getChats.SendAsync(fromDate, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetExecutionsAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getExecutions.SendAsync(productCode, count, before, after, cancellationToken);
    }

    public Task<CallResult<ProtocolRequest, ProtocolResponse>> GetTickerAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.SendAsync(productCode, cancellationToken);
    }
}
