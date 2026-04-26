using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Market;

public sealed class BitflyerNativeMarketSnapshotGateway : IBitflyerMarketSnapshotGateway
{
    private readonly IBitflyerPublicNativeApi _publicApi;

    public BitflyerNativeMarketSnapshotGateway(IBitflyerPublicNativeApi publicApi)
    {
        _publicApi = publicApi;
    }

    public Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetTickerAsync(
            new GetTickerRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
    }

    public Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetBoardStateAsync(
            new GetBoardStateRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
    }
}
