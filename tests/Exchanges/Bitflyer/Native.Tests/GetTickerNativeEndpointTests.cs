using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetTickerNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetTickerNativeEndpoint(new FakeGetTickerProtocolEndpoint(_ => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_ReturnsHttp_WhenStatusIsNotExpected()
    {
        var endpoint = new GetTickerNativeEndpoint(new FakeGetTickerProtocolEndpoint(_ => FailureProtocolCall(500, "{}")));
        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = null });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Http, call.Error!.Kind);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var body = """
        {
          "product_code": "BTC_JPY",
          "state": "RUNNING",
          "timestamp": "2015-07-08T02:50:59.97",
          "tick_id": 3579,
          "best_bid": 30000,
          "best_ask": 36640,
          "best_bid_size": 0.1,
          "best_ask_size": 5,
          "total_bid_depth": 15.13,
          "total_ask_depth": 20,
          "market_bid_size": 0,
          "market_ask_size": 0,
          "ltp": 31690,
          "volume": 16819.26,
          "volume_by_product": 6819.26
        }
        """;

        var endpoint = new GetTickerNativeEndpoint(new FakeGetTickerProtocolEndpoint(_ => SuccessProtocolCall(body)));
        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = "BTC_JPY" });

        Assert.True(call.IsSuccess);
        Assert.Equal("BTC_JPY", call.Response!.ProductCode);
        Assert.Equal(31690m, call.Response.Ltp);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_ReturnsCodec_WhenRequiredPropertyIsMissing()
    {
        var endpoint = new GetTickerNativeEndpoint(new FakeGetTickerProtocolEndpoint(_ => SuccessProtocolCall("""{"product_code":"BTC_JPY"}""")));
        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = null });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Codec, call.Error!.Kind);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetTicker", Method = "GET", Path = "/v1/getticker", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetTicker", Scope = "Public", Auth = "None" });
    }

    private static Call<ProtocolRequest, ProtocolResponse> FailureProtocolCall(int statusCode, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetTicker", Method = "GET", Path = "/v1/getticker", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = statusCode, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetTicker", Scope = "Public", Auth = "None" });
    }
}
