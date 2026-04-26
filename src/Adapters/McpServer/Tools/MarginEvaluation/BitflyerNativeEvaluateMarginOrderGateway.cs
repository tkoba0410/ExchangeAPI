using ExchangeApi.Exchanges.Bitflyer.Native.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tools.MarginEvaluation;

public sealed class BitflyerNativeEvaluateMarginOrderGateway : IBitflyerEvaluateMarginOrderGateway
{
    private readonly IBitflyerPublicNativeApi _publicApi;
    private readonly IBitflyerPrivateNativeApi _privateApi;

    public BitflyerNativeEvaluateMarginOrderGateway(
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

    public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralAsync(new GetCollateralRequest(), cancellationToken);
    }

    public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPositionsAsync(
            new GetPositionsRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
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

    public Task<CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageAsync(
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetCorporateLeverageAsync(
            new GetCorporateLeverageRequest(),
            cancellationToken);
    }
}
