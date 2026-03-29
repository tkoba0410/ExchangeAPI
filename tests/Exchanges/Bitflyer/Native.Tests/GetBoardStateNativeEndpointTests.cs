using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetBoardState;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetBoardStateNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetBoardStateNativeEndpoint(new FakeGetBoardStateProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetBoardStateRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var endpoint = new GetBoardStateNativeEndpoint(new FakeGetBoardStateProtocolEndpoint(_ => SuccessProtocolCall("""{"health":"NORMAL","state":"RUNNING","data":{"special_quotation":123}}""")));

        var call = await endpoint.CallAsync(new GetBoardStateRequest { ProductCode = "BTC_JPY" });

        Assert.True(call.IsSuccess);
        Assert.Equal(HealthStatuses.Normal, call.Response!.Health);
        Assert.Equal(TradingStates.Running, call.Response.State);
        Assert.Equal(123m, call.Response.Data!.SpecialQuotation);
    }

    [Fact]
    public async Task CallAsync_ReturnsCodec_WhenRequiredPropertyIsMissing()
    {
        var endpoint = new GetBoardStateNativeEndpoint(new FakeGetBoardStateProtocolEndpoint(_ => SuccessProtocolCall("""{"health":"NORMAL"}""")));

        var call = await endpoint.CallAsync(new GetBoardStateRequest());

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Codec, call.Error!.Kind);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetBoardState", Method = "GET", Path = "/v1/getboardstate", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetBoardState", Scope = "Public", Auth = "None" });
    }
}
