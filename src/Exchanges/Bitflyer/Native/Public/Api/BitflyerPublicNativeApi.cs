using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;

public sealed class BitflyerPublicNativeApi : IBitflyerPublicNativeApi
{
    private readonly IGetBoardNativeEndpoint _getBoard;
    private readonly IGetTickerNativeEndpoint _getTicker;

    public BitflyerPublicNativeApi(
        IGetBoardNativeEndpoint getBoard,
        IGetTickerNativeEndpoint getTicker)
    {
        _getBoard = getBoard;
        _getTicker = getTicker;
    }

    public Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        GetBoardRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.CallAsync(request, cancellationToken);
    }

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        GetTickerRequest request,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.CallAsync(request, cancellationToken);
    }
}
