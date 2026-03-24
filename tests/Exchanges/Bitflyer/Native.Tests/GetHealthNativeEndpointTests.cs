using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetHealth;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetHealthNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenProductCodeIsBlank()
    {
        var endpoint = new GetHealthNativeEndpoint(new FakeGetHealthProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetHealthRequest { ProductCode = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var endpoint = new GetHealthNativeEndpoint(new FakeGetHealthProtocolEndpoint(_ => SuccessProtocolCall("""{"status":"NORMAL"}""")));

        var call = await endpoint.CallAsync(new GetHealthRequest());

        Assert.True(call.IsSuccess);
        Assert.Equal("NORMAL", call.Response!.Status);
    }

    private static Call<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetHealth", Method = "GET", Path = "/v1/gethealth", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetHealth", Scope = "Public", Auth = "None" });
    }
}
