using ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Native.Tests;

public sealed class GetChatsNativeEndpointTests
{
    [Fact]
    public async Task CallAsync_ReturnsSemantic_WhenFromDateIsBlank()
    {
        var endpoint = new GetChatsNativeEndpoint(new FakeGetChatsProtocolEndpoint(_ => throw new InvalidOperationException()));

        var call = await endpoint.CallAsync(new GetChatsRequest { FromDate = " " });

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Semantic, call.Error!.Kind);
    }

    [Fact]
    public async Task CallAsync_MapsSuccessfulResponse()
    {
        var body = """
        [
          { "nickname": "alice", "message": "hello", "date": "2015-07-08T02:50:59.97" }
        ]
        """;

        var endpoint = new GetChatsNativeEndpoint(new FakeGetChatsProtocolEndpoint(_ => SuccessProtocolCall(body)));

        var call = await endpoint.CallAsync(new GetChatsRequest());

        Assert.True(call.IsSuccess);
        Assert.Single(call.Response!);
        Assert.Equal("alice", call.Response![0].Nickname);
        Assert.Equal("hello", call.Response[0].Message);
        Assert.Equal(new DateTimeOffset(2015, 7, 8, 2, 50, 59, 970, TimeSpan.Zero), call.Response[0].Date);
    }

    private static CallResult<ProtocolRequest, ProtocolResponse> SuccessProtocolCall(string bodyText)
    {
        return CallFactory.Success(
            new ProtocolRequest { EndpointId = "GetChats", Method = "GET", Path = "/v1/getchats", Query = null, BodyText = null },
            new ProtocolResponse { StatusCode = 200, Headers = new Dictionary<string, string[]>(), BodyText = bodyText },
            new CallMeta { Layer = CallLayers.Protocol, Component = CallComponents.PublicEndpointModule, EndpointId = "GetChats", Scope = "Public", Auth = "None" });
    }
}
