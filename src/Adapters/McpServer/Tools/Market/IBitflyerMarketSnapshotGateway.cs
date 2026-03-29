using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Market;

public interface IBitflyerMarketSnapshotGateway
{
    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        string symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
