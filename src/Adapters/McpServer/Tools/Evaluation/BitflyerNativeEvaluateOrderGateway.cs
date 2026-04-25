using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
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

    public Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetBalanceAsync(new GetBalanceRequest(), cancellationToken);
    }

    public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetChildOrdersAsync(
            new GetChildOrdersRequest
            {
                ProductCode = symbol,
                ChildOrderState = ChildOrderStates.Active,
            },
            cancellationToken);
    }
}
