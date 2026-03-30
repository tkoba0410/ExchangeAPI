using ExchangeApi.Adapters.McpServer.Schema.Evaluation;
using ExchangeApi.Adapters.McpServer.Tools.Evaluation;
using ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class EvaluateOrderToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceTrueForMarketBuy()
    {
        var tool = new EvaluateOrderTool(CreateHappyPathGateway(jpyAvailable: 5000000m, btcAvailable: 1m));

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = " btc_jpy ",
                Side = " BUY ",
                OrderType = " market ",
                Size = "0.300",
                Price = null,
            });

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateOrderResponse>(result.Response);
        Assert.True(response.CanPlace);
        Assert.True(response.Checks.SymbolOk);
        Assert.True(response.Checks.MarketStatusOk);
        Assert.True(response.Checks.SizeRuleOk);
        Assert.True(response.Checks.PriceRuleOk);
        Assert.True(response.Checks.BalanceOk);
        Assert.True(response.Checks.PositionLimitOk);
        Assert.Equal("BTC_JPY", response.NormalizedRequest.Symbol);
        Assert.Equal("buy", response.NormalizedRequest.Side);
        Assert.Equal("market", response.NormalizedRequest.OrderType);
        Assert.Equal("0.300", response.NormalizedRequest.Size);
        Assert.Null(response.NormalizedRequest.Price);
        Assert.Equal("12346000", response.Estimate.ReferencePrice);
        Assert.Equal("3703800.000", response.Estimate.EstimatedNotional);
        Assert.Equal(["market_order_slippage_risk"], response.Warnings);
        Assert.Empty(response.Reasons);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenSymbolIsUnsupported()
    {
        var tool = new EvaluateOrderTool(CreateHappyPathGateway());

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = "FX_BTC_JPY",
                Side = "buy",
                OrderType = "market",
                Size = "0.1",
                Price = null,
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_symbol", error.ErrorCode);
        Assert.False(error.Retryable);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenMarketOrderIncludesPrice()
    {
        var tool = new EvaluateOrderTool(CreateHappyPathGateway());

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = "BTC_JPY",
                Side = "buy",
                OrderType = "market",
                Size = "0.1",
                Price = "10000000",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_price", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceFalseForPriceRuleViolationAndInsufficientBalance()
    {
        var tool = new EvaluateOrderTool(CreateHappyPathGateway(jpyAvailable: 100000m, btcAvailable: 1m));

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = "BTC_JPY",
                Side = "buy",
                OrderType = "limit",
                Size = "0.100",
                Price = "12345678.5",
            });

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateOrderResponse>(result.Response);
        Assert.False(response.CanPlace);
        Assert.True(response.Checks.SymbolOk);
        Assert.True(response.Checks.MarketStatusOk);
        Assert.True(response.Checks.SizeRuleOk);
        Assert.False(response.Checks.PriceRuleOk);
        Assert.False(response.Checks.BalanceOk);
        Assert.True(response.Checks.PositionLimitOk);
        Assert.Equal(["price_rule_violation", "insufficient_balance"], response.Reasons);
        Assert.Empty(response.Warnings);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsCanPlaceFalseWhenMarketIsRestrictedAndExposureLimitIsExceeded()
    {
        var tool = new EvaluateOrderTool(
            CreateHappyPathGateway(
                boardState: new GetBoardStateResponse
                {
                    State = TradingStates.Starting,
                    Health = HealthStatuses.Normal,
                    Data = null,
                }),
            new EvaluateOrderOptions
            {
                MaxBaseSize = 0.2m,
            });

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = "BTC_JPY",
                Side = "sell",
                OrderType = "market",
                Size = "0.300",
                Price = null,
            });

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<EvaluateOrderResponse>(result.Response);
        Assert.False(response.CanPlace);
        Assert.False(response.Checks.MarketStatusOk);
        Assert.False(response.Checks.PositionLimitOk);
        Assert.Equal(["market_not_active", "exposure_limit_exceeded"], response.Reasons);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUpstreamErrorWhenBalanceFails()
    {
        var tool = new EvaluateOrderTool(
            CreateHappyPathGateway(
                balanceCall: CallFactory.Failure<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>(
                    new GetBalanceRequest(),
                    new CallError
                    {
                        Kind = CallErrorKinds.Http,
                        Message = "500",
                    },
                    TestCallMeta("GetBalance", "Private", "ApiKey"))));

        var result = await tool.ExecuteAsync(
            new EvaluateOrderRequest
            {
                Symbol = "BTC_JPY",
                Side = "buy",
                OrderType = "market",
                Size = "0.1",
                Price = null,
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("upstream_error", error.ErrorCategory);
        Assert.Equal("account_unavailable", error.ErrorCode);
        Assert.Equal("GetBalance", error.Details["endpoint"]);
        Assert.Equal("Http", error.Details["callErrorKind"]);
        Assert.True(error.Retryable);
    }

    private static FakeBitflyerEvaluateOrderGateway CreateHappyPathGateway(
        decimal jpyAvailable = 5000000m,
        decimal btcAvailable = 1m,
        GetBoardStateResponse? boardState = null,
        Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>? balanceCall = null)
    {
        return new FakeBitflyerEvaluateOrderGateway
        {
            TickerCall = CallFactory.Success(
                new GetTickerRequest { ProductCode = "BTC_JPY" },
                new GetTickerResponse
                {
                    ProductCode = "BTC_JPY",
                    State = TradingStates.Running,
                    Timestamp = new DateTimeOffset(2026, 03, 30, 10, 00, 00, TimeSpan.Zero),
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
                new GetBoardStateRequest { ProductCode = "BTC_JPY" },
                boardState ?? new GetBoardStateResponse
                {
                    State = TradingStates.Running,
                    Health = HealthStatuses.Normal,
                    Data = null,
                },
                TestCallMeta("GetBoardState", "Public", "None")),
            BalanceCall = balanceCall ?? CallFactory.Success(
                new GetBalanceRequest(),
                (IReadOnlyList<GetBalance.Item>)
                [
                    new GetBalance.Item { CurrencyCode = "JPY", Amount = jpyAvailable, Available = jpyAvailable },
                    new GetBalance.Item { CurrencyCode = "BTC", Amount = btcAvailable, Available = btcAvailable },
                ],
                TestCallMeta("GetBalance", "Private", "ApiKey")),
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

    private sealed class FakeBitflyerEvaluateOrderGateway : IBitflyerEvaluateOrderGateway
    {
        public required Call<GetTickerRequest, GetTickerResponse> TickerCall { get; init; }

        public required Call<GetBoardStateRequest, GetBoardStateResponse> BoardStateCall { get; init; }

        public required Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>> BalanceCall { get; init; }

        public Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(TickerCall);
        }

        public Task<Call<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateCallAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(BoardStateCall);
        }

        public Task<Call<GetBalanceRequest, IReadOnlyList<GetBalance.Item>>> GetBalanceCallAsync(
            CancellationToken cancellationToken = default)
        {
            _ = cancellationToken;
            return Task.FromResult(BalanceCall);
        }
    }
}
