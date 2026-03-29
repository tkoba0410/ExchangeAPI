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

    public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetTickerCallAsync(
            new GetTickerRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
    }

    public Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetBoardStateCallAsync(
            new GetBoardStateRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
    }
}
