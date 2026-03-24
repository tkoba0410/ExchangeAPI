using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public sealed class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IGetMarketsNativeEndpoint _getMarkets;
    private readonly IGetBoardNativeEndpoint _getBoard;
    private readonly IGetExecutionsPublicNativeEndpoint _getExecutions;
    private readonly IGetTickerNativeEndpoint _getTicker;

    public BitflyerPublicNativeApi(
        IGetMarketsNativeEndpoint getMarkets,
        IGetBoardNativeEndpoint getBoard,
        IGetExecutionsPublicNativeEndpoint getExecutions,
        IGetTickerNativeEndpoint getTicker)
    {
        _getMarkets = getMarkets;
        _getBoard = getBoard;
        _getExecutions = getExecutions;
        _getTicker = getTicker;
    }

    public Task<Call<GetMarketsRequest, IReadOnlyList<GetMarkets.Item>>> GetMarketsCallAsync(
        GetMarketsRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getMarkets.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetExecutionsPublicRequest, IReadOnlyList<GetExecutionsPublic.Item>>> GetExecutionsCallAsync(
        GetExecutionsPublicRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getExecutions.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.CallAsync(request, cancellationToken);
    }
}
