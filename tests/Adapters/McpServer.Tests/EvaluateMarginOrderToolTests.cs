using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.MarginEvaluation;
using ExchangeApi.Adapters.McpServer.Tools.MarginEvaluation;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class EvaluateMarginOrderToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceTrueForMarketBuy()
    {
        var tool = new EvaluateMarginOrderTool(CreateHappyPathGateway());

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: " fx_btc_jpy ",
                side: " BUY ",
                orderType: " market ",
                size: "0.300"));

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateMarginOrderResponse>(result.Response);
        Assert.True(response.CanPlace);
        Assert.True(response.Checks.SymbolOk);
        Assert.True(response.Checks.MarketStatusOk);
        Assert.True(response.Checks.SizeRuleOk);
        Assert.True(response.Checks.PriceRuleOk);
        Assert.True(response.Checks.CollateralCoverageOk);
        Assert.Null(response.Checks.FeeCoverageOk);
        Assert.True(response.Checks.ProjectedMarginExposureOk);
        Assert.True(response.Checks.CurrentMaintenanceOk);
        Assert.Equal(McpVenueIds.Bitflyer, response.NormalizedRequest.Venue);
        Assert.Equal(McpAccountContextIds.Default, response.NormalizedRequest.AccountContext);
        Assert.Equal("FX_BTC_JPY", response.NormalizedRequest.Symbol);
        Assert.Equal("buy", response.NormalizedRequest.Side);
        Assert.Equal("market", response.NormalizedRequest.OrderType);
        Assert.Equal("0.300", response.NormalizedRequest.Size);
        Assert.Null(response.NormalizedRequest.Price);
        Assert.Equal("12346000", response.Estimate.ReferencePrice);
        Assert.Equal("3703800.000", response.Estimate.EstimatedNotional);
        Assert.Equal("1851900.000", response.Estimate.EstimatedRequiredCollateral);
        Assert.Equal("2", response.Estimate.CurrentMaxLeverage);
        Assert.Equal("8", response.Estimate.CurrentKeepRate);
        Assert.Equal("1.2", response.Estimate.MinimumKeepRate);
        Assert.Null(response.Estimate.EstimatedFee);
        Assert.Null(response.Estimate.EstimatedFeeSourceKind);
        Assert.Equal([EvaluateMarginOrderWarningCodes.MarketOrderSlippageRisk], response.Warnings);
        Assert.Empty(response.Reasons);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenSymbolIsUnsupported()
    {
        var tool = new EvaluateMarginOrderTool(CreateHappyPathGateway());

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: "BTC_JPY",
                side: "buy",
                orderType: "market",
                size: "0.1"));

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_symbol", error.ErrorCode);
        Assert.False(error.Retryable);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceFalseForPriceRuleViolationAndInsufficientCollateral()
    {
        var tool = new EvaluateMarginOrderTool(
            CreateHappyPathGateway(
                collateral: new GetCollateralResponse
                {
                    Collateral = 200000m,
                    OpenPositionPnl = 0m,
                    RequireCollateral = 0m,
                    KeepRate = 8m,
                }));

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: "FX_BTC_JPY",
                side: "buy",
                orderType: "limit",
                size: "0.100",
                price: "12345678.5"));

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateMarginOrderResponse>(result.Response);
        Assert.False(response.CanPlace);
        Assert.True(response.Checks.SymbolOk);
        Assert.True(response.Checks.MarketStatusOk);
        Assert.True(response.Checks.SizeRuleOk);
        Assert.False(response.Checks.PriceRuleOk);
        Assert.False(response.Checks.CollateralCoverageOk);
        Assert.True(response.Checks.ProjectedMarginExposureOk);
        Assert.True(response.Checks.CurrentMaintenanceOk);
        Assert.Equal(["price_rule_violation", "insufficient_collateral"], response.Reasons);
        Assert.Empty(response.Warnings);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceFalseWhenMarketIsRestrictedExposureLimitExceededAndMaintenanceIsUnsafe()
    {
        var tool = new EvaluateMarginOrderTool(
            CreateHappyPathGateway(
                boardState: new GetBoardStateResponse
                {
                    State = TradingStates.Starting,
                    Health = HealthStatuses.Normal,
                    Data = null,
                },
                collateral: new GetCollateralResponse
                {
                    Collateral = 5_000_000m,
                    OpenPositionPnl = 0m,
                    RequireCollateral = 100_000m,
                    KeepRate = 1.1m,
                },
                positions:
                [
                    Position(OrderSides.Buy, size: 0.2m),
                ]),
            new EvaluateMarginOrderOptions
            {
                MaxBaseSize = 0.4m,
            });

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: "FX_BTC_JPY",
                side: "buy",
                orderType: "market",
                size: "0.300"));

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateMarginOrderResponse>(result.Response);
        Assert.False(response.CanPlace);
        Assert.False(response.Checks.MarketStatusOk);
        Assert.False(response.Checks.ProjectedMarginExposureOk);
        Assert.False(response.Checks.CurrentMaintenanceOk);
        Assert.Equal(["market_not_active", "exposure_limit_exceeded", "maintenance_not_safe"], response.Reasons);
    }

    [Fact]
    public async Task ExecuteAsync_ExposesFeeEstimateAndWarningWithoutBlockingCanPlace()
    {
        var tool = new EvaluateMarginOrderTool(
            CreateHappyPathGateway(
                collateral: new GetCollateralResponse
                {
                    Collateral = 1_853_000m,
                    OpenPositionPnl = 0m,
                    RequireCollateral = 0m,
                    KeepRate = 2m,
                }),
            new EvaluateMarginOrderOptions
            {
                MarketFeeRate = 0.001m,
            });

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: "FX_BTC_JPY",
                side: "buy",
                orderType: "market",
                size: "0.300"));

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateMarginOrderResponse>(result.Response);
        Assert.True(response.CanPlace);
        Assert.Equal(false, response.Checks.FeeCoverageOk);
        Assert.Equal("3703.800000", response.Estimate.EstimatedFee);
        Assert.Equal(MarketRuleSourceKinds.PinnedOperational, response.Estimate.EstimatedFeeSourceKind);
        Assert.Equal(
            [EvaluateMarginOrderWarningCodes.MarketOrderSlippageRisk, EvaluateMarginOrderWarningCodes.EstimatedFeeNotCovered],
            response.Warnings);
        Assert.Empty(response.Reasons);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUpstreamErrorWhenCollateralFails()
    {
        var tool = new EvaluateMarginOrderTool(
            CreateHappyPathGateway(
                collateralCall: CallFactory.Failure<GetCollateralRequest, GetCollateralResponse>(
                    new GetCollateralRequest(),
                    new CallError
                    {
                        Kind = CallErrorKinds.Http,
                        Message = "500",
                    },
                    TestCallMeta("GetCollateral", "Private", "ApiKey"))));

        var result = await tool.ExecuteAsync(
            CreateRequest(
                symbol: "FX_BTC_JPY",
                side: "buy",
                orderType: "market",
                size: "0.1"));

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("upstream_error", error.ErrorCategory);
        Assert.Equal("account_unavailable", error.ErrorCode);
        Assert.Equal("GetCollateral", error.Details["endpoint"]);
        Assert.Equal("Http", error.Details["callErrorKind"]);
        Assert.True(error.Retryable);
    }

    private static FakeBitflyerEvaluateMarginOrderGateway CreateHappyPathGateway(
        GetBoardStateResponse? boardState = null,
        GetCollateralResponse? collateral = null,
        CallResult<GetCollateralRequest, GetCollateralResponse>? collateralCall = null,
        IReadOnlyList<GetPositions.Item>? positions = null,
        IReadOnlyList<GetChildOrders.Item>? activeOrders = null,
        GetCorporateLeverageResponse? corporateLeverage = null)
    {
        return new FakeBitflyerEvaluateMarginOrderGateway
        {
            TickerCall = CallFactory.Success(
                new GetTickerRequest { ProductCode = "FX_BTC_JPY" },
                new GetTickerResponse
                {
                    ProductCode = "FX_BTC_JPY",
                    State = TradingStates.Running,
                    Timestamp = new DateTimeOffset(2026, 03, 31, 10, 00, 00, TimeSpan.Zero),
                    TickId = 1,
                    BestBid = 12345000m,
                    BestAsk = 12346000m,
                    BestBidSize = 1m,
                    BestAskSize = 1m,
                    TotalBidDepth = 1m,
                    TotalAskDepth = 1m,
                    MarketBidSize = 0m,
                    MarketAskSize = 0m,
                    Ltp = 12345500m,
                    Volume = 10m,
                    VolumeByProduct = 10m,
                },
                TestCallMeta("GetTicker", "Public", "None")),
            BoardStateCall = CallFactory.Success(
                new GetBoardStateRequest { ProductCode = "FX_BTC_JPY" },
                boardState ?? new GetBoardStateResponse
                {
                    State = TradingStates.Running,
                    Health = HealthStatuses.Normal,
                    Data = null,
                },
                TestCallMeta("GetBoardState", "Public", "None")),
            CollateralCall = collateralCall ?? CallFactory.Success(
                new GetCollateralRequest(),
                collateral ?? new GetCollateralResponse
                {
                    Collateral = 5_000_000m,
                    OpenPositionPnl = 100_000m,
                    RequireCollateral = 600_000m,
                    KeepRate = 8m,
                },
                TestCallMeta("GetCollateral", "Private", "ApiKey")),
            PositionsCall = CallFactory.Success(
                new GetPositionsRequest { ProductCode = "FX_BTC_JPY" },
                positions ?? Array.Empty<GetPositions.Item>(),
                TestCallMeta("GetPositions", "Private", "ApiKey")),
            ActiveOrdersCall = CallFactory.Success(
                new GetChildOrdersRequest
                {
                    ProductCode = "FX_BTC_JPY",
                    ChildOrderState = ChildOrderStates.Active,
                },
                activeOrders ?? Array.Empty<GetChildOrders.Item>(),
                TestCallMeta("GetChildOrders", "Private", "ApiKey")),
            CorporateLeverageCall = CallFactory.Success(
                new GetCorporateLeverageRequest(),
                corporateLeverage ?? new GetCorporateLeverageResponse
                {
                    CurrentMax = 2m,
                    CurrentStartDate = new DateTimeOffset(2026, 03, 01, 0, 0, 0, TimeSpan.Zero),
                    NextMax = null,
                    NextStartDate = null,
                },
                TestCallMeta("GetCorporateLeverage", "Public", "None")),
        };
    }

    private static GetPositions.Item Position(BitflyerOrderSide side, decimal size)
    {
        return new GetPositions.Item
        {
            ProductCode = "FX_BTC_JPY",
            Side = side,
            Price = 12345000m,
            Size = size,
            Commission = 0m,
            SwapPointAccumulate = 0m,
            RequireCollateral = 100000m,
            OpenDate = new DateTimeOffset(2026, 03, 30, 10, 0, 0, TimeSpan.Zero),
            Leverage = 2m,
            Pnl = 0m,
            Sfd = 0m,
        };
    }

    private static EvaluateMarginOrderRequest CreateRequest(
        string symbol,
        string side,
        string orderType,
        string size,
        string? price = null,
        string venue = McpVenueIds.Bitflyer,
        string accountContext = McpAccountContextIds.Default)
    {
        return new EvaluateMarginOrderRequest
        {
            Venue = venue,
            AccountContext = accountContext,
            Symbol = symbol,
            Side = side,
            OrderType = orderType,
            Size = size,
            Price = price,
        };
    }

    private static CallMeta TestCallMeta(string endpointId, string scope, string auth)
    {
        return new CallMeta
        {
            Layer = CallLayers.Tests,
            Component = CallComponents.Factory,
            EndpointId = endpointId,
            Scope = scope,
            Auth = auth,
            Children = null,
        };
    }

    private sealed class FakeBitflyerEvaluateMarginOrderGateway : IBitflyerEvaluateMarginOrderGateway
    {
        public required CallResult<GetTickerRequest, GetTickerResponse> TickerCall { get; init; }

        public required CallResult<GetBoardStateRequest, GetBoardStateResponse> BoardStateCall { get; init; }

        public required CallResult<GetCollateralRequest, GetCollateralResponse> CollateralCall { get; init; }

        public required CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>> PositionsCall { get; init; }

        public required CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>> ActiveOrdersCall { get; init; }

        public required CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse> CorporateLeverageCall { get; init; }

        public Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(TickerCall);
        }

        public Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(BoardStateCall);
        }

        public Task<CallResult<GetCollateralRequest, GetCollateralResponse>> GetCollateralAsync(
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(CollateralCall);
        }

        public Task<CallResult<GetPositionsRequest, IReadOnlyList<GetPositions.Item>>> GetPositionsAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(PositionsCall);
        }

        public Task<CallResult<GetChildOrdersRequest, IReadOnlyList<GetChildOrders.Item>>> GetActiveChildOrdersAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(ActiveOrdersCall);
        }

        public Task<CallResult<GetCorporateLeverageRequest, GetCorporateLeverageResponse>> GetCorporateLeverageAsync(
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(CorporateLeverageCall);
        }
    }
}
