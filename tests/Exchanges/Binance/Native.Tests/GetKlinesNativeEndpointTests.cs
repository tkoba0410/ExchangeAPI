using ExchangeApi.Exchanges.Binance.Native.Public.Endpoints.GetKlines;
using ExchangeApi.Exchanges.Binance.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Binance.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Binance.Native.Tests;

public sealed class GetKlinesNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenIntervalIsInvalid()
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = default });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenLimitIsOutOfRange()
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h, Limit = 1001 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenTimeZoneIsInvalid()
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h, TimeZone = "15:00" });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("8")]
    [InlineData("4")]
    [InlineData("-1:00")]
    [InlineData("05:45")]
    public async Task CallAsync_AcceptsOfficiallyDocumentedTimeZoneExamples(string timeZone)
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => SuccessProtocolCall(200, "[]")));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h, TimeZone = timeZone });

        Assert.True(call.IsSuccess);
    }

    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenStartTimeExceedsEndTime()
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h, StartTime = 2, EndTime = 1 });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_ReturnsHttp_WhenStatusIsNotExpected()
    {
        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => SuccessProtocolCall(429, "[]")));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Http, call.Error!.Kind);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var body = """
        [
          [
            1499040000000,
            "0.01634790",
            "0.80000000",
            "0.01575800",
            "0.01577100",
            "148976.11427815",
            1499644799999,
            "2434.19055334",
            308,
            "1756.87402397",
            "28.46694368",
            "0"
          ]
        ]
        """;

        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => SuccessProtocolCall(200, body)));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h, Limit = 1 });

        Assert.True(call.IsSuccess);
        var item = Assert.Single(call.Response!);
        Assert.Equal(1499040000000L, item.OpenTime);
        Assert.Equal(0.01634790m, item.OpenPrice);
        Assert.Equal(308, item.NumberOfTrades);
    }

    [Fact]
    public async Task CallAsync_ReturnsCodec_WhenTupleLengthIsInvalid()
    {
        var body = """
        [
          [1499040000000, "0.01634790"]
        ]
        """;

        var endpoint = new GetKlinesNativeEndpoint(new FakeGetKlinesProtocolEndpoint((_, _, _, _, _, _) => SuccessProtocolCall(200, body)));
        var call = await endpoint.CallAsync(new GetKlinesRequest { Symbol = "BTCJPY", Interval = BinanceIntervals.Hour1h });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Codec, call.Error!.Kind);
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(int statusCode, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest
            {
                EndpointId = "GetKlines",
                Method = "GET",
                Path = "/api/v3/klines",
                Query = null,
                BodyText = null,
            },
            new ProtocolResponse
            {
                StatusCode = statusCode,
                Headers = new Dictionary<string, string[]>(),
                BodyText = bodyText,
            },
            new CallMeta
            {
                Layer = CallLayers.Protocol,
                Component = CallComponents.PublicEndpointModule,
                EndpointId = "GetKlines",
                Scope = "Public",
                Auth = "None",
            });
    }
}
