using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoard;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Api;

public sealed class BitflyerPublicProtocolApi : IBitflyerPublicProtocolApi
{
    private readonly IGetBoardProtocolEndpoint _getBoard;
    private readonly IGetTickerProtocolEndpoint _getTicker;

    public BitflyerPublicProtocolApi(
        IGetBoardProtocolEndpoint getBoard,
        IGetTickerProtocolEndpoint getTicker)
    {
        _getBoard = getBoard;
        _getTicker = getTicker;
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetBoardCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getBoard.SendAsync(productCode, cancellationToken);
    }

    public Task<Call<ProtocolRequest, ProtocolResponse>> GetTickerCallAsync(
        string? productCode,
        CancellationToken cancellationToken = default)
    {
        return _getTicker.SendAsync(productCode, cancellationToken);
    }
}
