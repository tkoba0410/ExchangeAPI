using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public sealed class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IGetMarketsProtocolEndpoint _getMarkets;
    private readonly IGetBoardProtocolEndpoint _getBoard;
    private readonly IGetExecutionsPublicProtocolEndpoint _getExecutions;
    private readonly IGetTickerProtocolEndpoint _getTicker;

    public BitflyerPublicProtocolApi(
        IGetMarketsProtocolEndpoint getMarkets,
        IGetBoardProtocolEndpoint getBoard,
        IGetExecutionsPublicProtocolEndpoint getExecutions,
        IGetTickerProtocolEndpoint getTicker)
    {
        _getMarkets = getMarkets;
        _getBoard = getBoard;
        _getExecutions = getExecutions;
        _getTicker = getTicker;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetMarketsCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _getMarkets.SendAsync(cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBoardCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.SendAsync(productCode, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetExecutionsCallAsync(
        string? productCode,
        int? count,
        long? before,
        long? after,
        CancellationToken cancellationToken = default)
    {
        return _getExecutions.SendAsync(productCode, count, before, after, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.SendAsync(productCode, cancellationToken);
    }
}
