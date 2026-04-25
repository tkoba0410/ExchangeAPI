using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Market;

public interface IBitflyerMarketSnapshotGateway
{
    Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
