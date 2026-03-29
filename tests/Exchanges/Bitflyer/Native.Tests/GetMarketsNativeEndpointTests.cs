using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetMarketsNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsHttp_WhenStatusIsNotExpected()
    {
        var endpoint = new GetMarketsNativeEndpoint(new FakeGetMarketsProtocolEndpoint(() => FailureProtocolCall(500, "[]")));
        var call = await endpoint.CallAsync(new GetMarketsRequest());

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Http, call.Error!.Kind);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_MapsTopLevelArray()
    {
        var body = """
        [
          { "product_code": "BTC_JPY", "market_type": "Spot" },
          { "product_code": "FX_BTC_JPY", "market_type": "FX" }
        ]
        """;

        var endpoint = new GetMarketsNativeEndpoint(new FakeGetMarketsProtocolEndpoint(() => SuccessProtocolCall(body)));
        var call = await endpoint.CallAsync(new GetMarketsRequest());

        Assert.True(call.IsSuccess);
        Assert.Equal(2, call.Response!.Count);
        Assert.Equal("BTC_JPY", call.Response![0].ProductCode);
        Assert.Equal(MarketTypes.Fx, call.Response[1].MarketType);
    }

    [Fact]
    public async Task CallAsync_ReturnsCodec_WhenRequiredPropertyIsMissing()
    {
        var endpoint = new GetMarketsNativeEndpoint(new FakeGetMarketsProtocolEndpoint(() => SuccessProtocolCall("""[{"product_code":"BTC_JPY"}]""")));
        var call = await endpoint.CallAsync(new GetMarketsRequest());

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Codec, call.Error!.Kind);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetMarkets", Method = "GET", Path = "/v1/getmarkets", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetMarkets", Scope = "Public", Auth = "None" });
    }

    private static Call<ProtocolRequest, ProtocolResponse> FailureProtocolCall(int statusCode, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetMarkets", Method = "GET", Path = "/v1/getmarkets", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = statusCode, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetMarkets", Scope = "Public", Auth = "None" });
    }
}
