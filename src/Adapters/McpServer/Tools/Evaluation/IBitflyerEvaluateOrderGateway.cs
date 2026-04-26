using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.Evaluation;

public interface IBitflyerEvaluateOrderGateway
{
    Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
        string symbol,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
        string symbol,
        CancellationToken cancellationToken = default);

    Task<CallResult<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceAsync(
        CancellationToken cancellationToken = default);

    Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersAsync(
        string symbol,
        CancellationToken cancellationToken = default);
}
