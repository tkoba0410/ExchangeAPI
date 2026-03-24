using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetBoardState;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetBoardStateProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_OmitsQuery_WhenProductCodeIsNull()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBoardStateProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(null);

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getboardstate", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Null(transport.LastRequest.Query);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }

    [Fact]
    public async Task SendAsync_SetsQuery_WhenProductCodeIsPresent()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBoardStateProtocolEndpoint(transport);

        await endpoint.SendAsync("BTC_JPY");

        Assert.NotNull(transport.LastRequest);
        Assert.NotNull(transport.LastRequest!.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
    }
}
