using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoard;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetBoardNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetBoardNativeEndpoint(new FakeGetBoardProtocolEndpoint(_ => throw new InvalidOperationException()));
        var call = await endpoint.CallAsync(new GetBoardRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_ReturnsHttp_WhenStatusIsNotExpected()
    {
        var endpoint = new GetBoardNativeEndpoint(new FakeGetBoardProtocolEndpoint(_ => FailureProtocolCall(500, "{}")));
        var call = await endpoint.CallAsync(new GetBoardRequest { ProductCode = null });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Http, call.Error!.Kind);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var body = """
        {
          "mid_price": 33320,
          "bids": [
            { "price": 30000, "size": 0.1 },
            { "price": 25570, "size": 3 }
          ],
          "asks": [
            { "price": 36640, "size": 5 },
            { "price": 36700, "size": 1.2 }
          ]
        }
        """;

        var endpoint = new GetBoardNativeEndpoint(new FakeGetBoardProtocolEndpoint(_ => SuccessProtocolCall(body)));
        var call = await endpoint.CallAsync(new GetBoardRequest { ProductCode = "BTC_JPY" });

        Assert.True(call.IsSuccess);
        Assert.Equal(33320m, call.Response!.MidPrice);
        Assert.Equal(2, call.Response.Bids.Count);
        Assert.Equal(30000m, call.Response.Bids[0].Price);
        Assert.Equal(1.2m, call.Response.Asks[1].Size);
        Assert.NotNull(call.Meta.Children);
    }

    [Fact]
    public async Task CallAsync_ReturnsCodec_WhenRequiredPropertyIsMissing()
    {
        var endpoint = new GetBoardNativeEndpoint(new FakeGetBoardProtocolEndpoint(_ => SuccessProtocolCall("""{"mid_price":33320}""")));
        var call = await endpoint.CallAsync(new GetBoardRequest { ProductCode = null });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Codec, call.Error!.Kind);
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetBoard", Method = "GET", Path = "/v1/getboard", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoard", Scope = "Public", Auth = "None" });
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> FailureProtocolCall(int statusCode, string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetBoard", Method = "GET", Path = "/v1/getboard", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = statusCode, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoard", Scope = "Public", Auth = "None" });
    }
}
