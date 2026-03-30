using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Evaluation;

public sealed class BitflyerNativeEvaluateOrderGateway : IBitflyerEvaluateOrderGateway
{
    private readonly IBitflyerPublicNativeApi _publicApi;
    private readonly IBitflyerPrivateNativeApi _privateApi;

    public BitflyerNativeEvaluateOrderGateway(
        IBitflyerPublicNativeApi publicApi,
        IBitflyerPrivateNativeApi privateApi)
    {
        _publicApi = publicApi;
        _privateApi = privateApi;
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

    public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetBalanceCallAsync(new GetBalanceRequest(), cancellationToken);
    }
}
