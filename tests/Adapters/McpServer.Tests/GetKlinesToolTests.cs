using ExchangeApi.Adapters.McpServer.Schema.Klines;
using ExchangeApi.Adapters.McpServer.Tools.Klines;
using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;
using BinanceGetKlinesRequest = ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines.GetKlinesRequest;
using McpGetKlinesRequest = ExchangeApi.Adapters.McpServer.Schema.Klines.GetKlinesRequest;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class GetKlinesToolTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsMappedCandlesForSupportedSymbol()
    {
        var tool = new GetKlinesTool(
            new FakeBinanceKlinesGateway
            {
                KlinesCall = CallFactory.Success(
                    new BinanceGetKlinesRequest
                    {
                        Symbol = "BTCUSDT",
                        Interval = BinanceIntervals.Hour1h,
                        StartTime = null,
                        EndTime = null,
                        TimeZone = null,
                        Limit = 2,
                    },
                    (IReadOnlyList<GetKlines.Item>)
                    [
                        new GetKlines.Item
                        {
                            OpenTime = 1_743_292_800_000L,
                            OpenPrice = 10700000m,
                            HighPrice = 10750000m,
                            LowPrice = 10680000m,
                            ClosePrice = 10720000m,
                            Volume = 123.45m,
                            CloseTime = 1_743_296_399_999L,
                            QuoteAssetVolume = 1323000000m,
                            NumberOfTrades = 12345,
                            TakerBuyBaseAssetVolume = 61.72m,
                            TakerBuyQuoteAssetVolume = 662100000m,
                        },
                    ],
                    TestCallMeta("GetKlines")),
            });

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = " binance ",
                Symbol = " btcusdt ",
                Interval = "1h",
                StartTime = null,
                EndTime = null,
                Limit = 2,
            });

        Assert.True(result.IsSuccess);
        var response = Assert.IsType<GetKlinesResponse>(result.Response);
        Assert.Equal("binance", response.Venue);
        Assert.Equal("BTCUSDT", response.Symbol);
        Assert.Equal("1h", response.Interval);
        var candle = Assert.Single(response.Candles);
        Assert.Equal("2025-03-30T00:00:00Z", candle.OpenTime);
        Assert.Equal("2025-03-30T00:59:59.999Z", candle.CloseTime);
        Assert.Equal("10700000", candle.Open);
        Assert.Equal("10750000", candle.High);
        Assert.Equal("10680000", candle.Low);
        Assert.Equal("10720000", candle.Close);
        Assert.Equal("123.45", candle.Volume);
        Assert.Equal("1323000000", candle.QuoteVolume);
        Assert.Equal(12345, candle.TradeCount);
        Assert.Equal("61.72", candle.TakerBuyBaseVolume);
        Assert.Equal("662100000", candle.TakerBuyQuoteVolume);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenSymbolIsUnsupported()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "SOLJPY",
                Interval = "1h",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_symbol", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenIntervalIsUnsupported()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "10h",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_interval", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenLimitIsOutOfRange()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "1h",
                Limit = 1001,
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_limit", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenStartTimeIsAfterEndTime()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "1h",
                StartTime = "2026-03-30T01:00:00Z",
                EndTime = "2026-03-30T00:00:00Z",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_time_range", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenVenueIsUnsupported()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "bitflyer",
                Symbol = "BTCUSDT",
                Interval = "1h",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_venue", error.ErrorCode);
    }

    [Fact]
    public async Task ExecuteAsync_NormalizesOffsetTimestampInputToUtc()
    {
        IReadOnlyList<GetKlines.Item> emptyItems = Array.Empty<GetKlines.Item>();

        var tool = new GetKlinesTool(
            new FakeBinanceKlinesGateway
            {
                KlinesCall = CallFactory.Success(
                    new BinanceGetKlinesRequest
                    {
                        Symbol = "BTCUSDT",
                        Interval = BinanceIntervals.Hour1h,
                        StartTime = 1_743_292_800_000L,
                        EndTime = 1_743_296_400_000L,
                        TimeZone = null,
                        Limit = 2,
                    },
                    emptyItems,
                    TestCallMeta("GetKlines")),
            });

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "1h",
                StartTime = "2025-03-30T09:00:00+09:00",
                EndTime = "2025-03-30T19:00:00+09:00",
                Limit = 2,
            });

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenTimestampHasNoExplicitOffset()
    {
        var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "1h",
                StartTime = "2025-03-30T09:00:00",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("validation_error", error.ErrorCategory);
        Assert.Equal("invalid_time_range", error.ErrorCode);
        Assert.Equal("2025-03-30T09:00:00", error.Details["timestamp"]);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsValidationErrorWhenVenueIsMissingFromContract()
    {
        var json = """{"symbol":"BTCUSDT","interval":"1h"}""";

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(
            async () =>
            {
                var request = System.Text.Json.JsonSerializer.Deserialize<McpGetKlinesRequest>(json)!;
                var tool = new GetKlinesTool(new FakeBinanceKlinesGateway());
                await tool.ExecuteAsync(request);
            });
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsUpstreamErrorWhenGetKlinesFails()
    {
        var tool = new GetKlinesTool(
            new FakeBinanceKlinesGateway
            {
                KlinesCall = CallFactory.Failure<BinanceGetKlinesRequest, IReadOnlyList<GetKlines.Item>>(
                    new BinanceGetKlinesRequest
                    {
                        Symbol = "BTCUSDT",
                        Interval = BinanceIntervals.Hour1h,
                    },
                    new CallError
                    {
                        Kind = CallErrorKinds.Transport,
                        Message = "network timeout",
                    },
                    TestCallMeta("GetKlines")),
            });

        var result = await tool.ExecuteAsync(
            new McpGetKlinesRequest
            {
                Venue = "binance",
                Symbol = "BTCUSDT",
                Interval = "1h",
            });

        Assert.False(result.IsSuccess);
        var error = Assert.IsType<ExchangeApi.Adapters.McpServer.Schema.McpToolError>(result.Error);
        Assert.Equal("upstream_error", error.ErrorCategory);
        Assert.Equal("market_unavailable", error.ErrorCode);
        Assert.Equal("GetKlines", error.Details["endpoint"]);
        Assert.True(error.Retryable);
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

    private sealed class FakeBinanceKlinesGateway : IBinanceKlinesGateway
    {
        public CallResult<BinanceGetKlinesRequest, IReadOnlyList<GetKlines.Item>>? KlinesCall { get; init; }

        public Task<CallResult<BinanceGetKlinesRequest, IReadOnlyList<GetKlines.Item>>> GetKlinesAsync(
            BinanceGetKlinesRequest request,
            CancellationToken cancellationToken = default)
        {
            _ = request;
            _ = cancellationToken;
            return Task.FromResult(
                KlinesCall ?? throw new InvalidOperationException("KlinesCall must be configured."));
        }
    }
}
