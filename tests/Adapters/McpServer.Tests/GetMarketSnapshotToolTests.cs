using ExchangeApi.Adapters.McpServer.Mapping;
using ExchangeApi.Adapters.McpServer.Schema.Market;
using ExchangeApi.Adapters.McpServer.Tools.Market;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class GetMarketSnapshotToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsSnapshotUsingTickerBoardStateAndRegistry()
    {
        var gateway = new FakeBitflyerMarketSnapshotGateway
        {
            TickerCall = CallFactory.Success(
                new GetTickerRequest { ProductCode = "BTC_JPY" },
                new GetTickerResponse
                {
                    ProductCode = "BTC_JPY",
                    State = TradingStates.Running,
                    Timestamp = new DateTimeOffset(2026, 03, 29, 10, 00, 00, TimeSpan.Zero),
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
                TestCallMeta("GetTicker")),
            BoardStateCall = CallFactory.Success(
                new GetBoardStateRequest { ProductCode = "BTC_JPY" },
                new GetBoardStateResponse
                {
                    State = TradingStates.Running,
                    Health = HealthStatuses.Normal,
                    Data = null,
                },
                TestCallMeta("GetBoardState")),
        };
        var tool = new GetMarketSnapshotTool(gateway);

        var result = await tool.ExecuteAsync(
            new GetMarketSnapshotRequest
            {
                Symbol = " btc_jpy ",
            });

        Assert.True(result.IsSuccess);
        Assert.Null(result.Error);
        var response = Assert.IsType<GetMarketSnapshotResponse>(result.Response);
        Assert.Equal("BTC_JPY", response.Symbol);
        Assert.Equal("12345000", response.Bid);
        Assert.Equal("12346000", response.Ask);
        Assert.Equal("12345500", response.Last);
        Assert.Equal("2026-03-29T10:00:00Z", response.Timestamp);
        Assert.Equal("0.001", response.Rules.MinSize);
        Assert.Equal("0.00000001", response.Rules.SizeStep);
        Assert.Equal("1", response.Rules.PriceStep);
        Assert.Equal(MarketRuleSourceKinds.OfficialDocumented, response.Rules.MinSizeSourceKind);
        Assert.Equal("https://bitflyer.com/ja-jp/s/commission", response.Rules.MinSizeSourceRef);
        Assert.Equal(MarketRuleSourceKinds.OfficialDocumented, response.Rules.SizeStepSourceKind);
        Assert.Equal("https://bitflyer.com/ja-jp/s/commission", response.Rules.SizeStepSourceRef);
        Assert.Equal(MarketRuleSourceKinds.AdapterInferred, response.Rules.PriceStepSourceKind);
        Assert.Equal("adapter://bitflyer-jpy-price-step.v1", response.Rules.PriceStepSourceRef);
        Assert.Equal("active", response.Status);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenSymbolIsUnsupported()
    {
        var tool = new GetMarketSnapshotTool(new FakeBitflyerMarketSnapshotGateway());

        var result = await tool.ExecuteAsync(
            new GetMarketSnapshotRequest
            {
                Symbol = "ETH_JPY",
            });

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_symbol", error.ErrorCode);
        Assert.False(error.Retryable);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUpstreamErrorWhenBoardStateFails()
    {
        var gateway = new FakeBitflyerMarketSnapshotGateway
        {
            TickerCall = CallFactory.Success(
                new GetTickerRequest { ProductCode = "BTC_JPY" },
                new GetTickerResponse
                {
                    ProductCode = "BTC_JPY",
                    State = TradingStates.Running,
                    Timestamp = new DateTimeOffset(2026, 03, 29, 10, 00, 00, TimeSpan.Zero),
                    TickId = 1,
                    BestBid = 1m,
                    BestAsk = 2m,
                    BestBidSize = 1m,
                    BestAskSize = 1m,
                    TotalBidDepth = 1m,
                    TotalAskDepth = 1m,
                    MarketBidSize = 0m,
                    MarketAskSize = 0m,
                    Ltp = 1.5m,
                    Volume = 10m,
                    VolumeByProduct = 10m,
                },
                TestCallMeta("GetTicker")),
            BoardStateCall = CallFactory.Failure<GetBoardStateRequest, GetBoardStateResponse>(
                new GetBoardStateRequest { ProductCode = "BTC_JPY" },
                new CallError
                {
                    Kind = CallErrorKinds.Transport,
                    Message = "network timeout",
                },
                TestCallMeta("GetBoardState")),
        };
        var tool = new GetMarketSnapshotTool(gateway);

        var result = await tool.ExecuteAsync(
            new GetMarketSnapshotRequest
            {
                Symbol = "BTC_JPY",
            });

        Assert.False(result.IsSuccess);
        Assert.Null(result.Response);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("upstream_error", error.ErrorCategory);
        Assert.Equal("market_unavailable", error.ErrorCode);
        Assert.True(error.Retryable);
        Assert.Equal("GetBoardState", error.Details["endpoint"]);
        Assert.Equal("Transport", error.Details["callErrorKind"]);
    }

    [Theory]
    [InlineData(TradingStates.Running, HealthStatuses.Busy, "active")]
    [InlineData(TradingStates.Starting, HealthStatuses.Normal, "restricted")]
    [InlineData(TradingStates.Running, HealthStatuses.VeryBusy, "restricted")]
    [InlineData(TradingStates.Closed, HealthStatuses.Busy, "halted")]
    [InlineData(TradingStates.Running, HealthStatuses.Stop, "halted")]
    public void BitflyerMarketStatusMapper_MapsRepresentativeStates(
        BitflyerTradingState state,
        BitflyerHealthStatus health,
        string expected)
    {
        var actual = BitflyerMarketStatusMapper.Map(state, health);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BitflyerMarketStatusMapper_ReturnsUnknownForUnmappedValues()
    {
        var actual = BitflyerMarketStatusMapper.Map((BitflyerTradingState)0, (BitflyerHealthStatus)0);

        Assert.Equal("unknown", actual);
    }

    private static CallMeta TestCallMeta(string endpointId)
    {
        return new CallMeta
        {
            Layer = CallLayers.Tests,
            Component = CallComponents.Factory,
            EndpointId = endpointId,
            Scope = "Public",
            Auth = "None",
            Children = null,
        };
    }

    private sealed class FakeBitflyerMarketSnapshotGateway : IBitflyerMarketSnapshotGateway
    {
        public CallResult<GetTickerRequest, GetTickerResponse>? TickerCall { get; init; }

        public CallResult<GetBoardStateRequest, GetBoardStateResponse>? BoardStateCall { get; init; }

        public Task<CallResult<GetTickerRequest, GetTickerResponse>> GetTickerAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(
                TickerCall ?? throw new InvalidOperationException("TickerCall must be configured."));
        }

        public Task<CallResult<GetBoardStateRequest, GetBoardStateResponse>> GetBoardStateAsync(
            string symbol,
            CancellationToken cancellationToken = default)
        {
            _ = symbol;
            _ = cancellationToken;
            return Task.FromResult(
                BoardStateCall ?? throw new InvalidOperationException("BoardStateCall must be configured."));
        }
    }
}
