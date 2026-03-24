using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetChats;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetChatsProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_OmitsQuery_WhenFromDateIsNull()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetChatsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(null);

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getchats", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Null(transport.LastRequest.Query);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }

    [Fact]
    public async Task SendAsync_SetsQuery_WhenFromDateIsPresent()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetChatsProtocolEndpoint(transport);

        await endpoint.SendAsync("2015-07-08T00:00:00");

        Assert.NotNull(transport.LastRequest);
        Assert.NotNull(transport.LastRequest!.Query);
        Assert.Equal("2015-07-08T00:00:00", transport.LastRequest.Query!["from_date"]);
    }
}
