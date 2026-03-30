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

    public Task<Call<GetCollateralRequest, GetCollateralResponse>> GetCollateralCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetCollateralCallAsync(new GetCollateralRequest(), cancellationToken);
    }

    public Task<Call<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsCallAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetPositionsCallAsync(
            new GetPositionsRequest
            {
                ProductCode = symbol,
            },
            cancellationToken);
    }

    public Task<Call<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersCallAsync(
        string symbol,
        CancellationToken cancellationToken = default)
    {
        return _privateApi.GetChildOrdersCallAsync(
            new GetChildOrdersRequest
            {
                ProductCode = symbol,
                ChildOrderState = ChildOrderStates.Active,
            },
            cancellationToken);
    }

    public Task<Call<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageCallAsync(
        CancellationToken cancellationToken = default)
    {
        return _publicApi.GetCorporateLeverageCallAsync(
            new GetCorporateLeverageRequest(),
            cancellationToken);
    }
}
