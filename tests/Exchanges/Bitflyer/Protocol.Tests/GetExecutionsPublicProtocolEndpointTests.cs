using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetExecutionsPublic;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetExecutionsPublicProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_UsesPublicGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetExecutionsPublicProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTC_JPY", 10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getexecutions", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Equal("10", transport.LastRequest.Query["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
    }

    [Fact]
    public async Task SendAsync_OmitsOptionalQuery_WhenValuesAreNull()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetExecutionsPublicProtocolEndpoint(transport);

        await endpoint.SendAsync(null, null, null, null);

        Assert.NotNull(transport.LastRequest);
        Assert.Null(transport.LastRequest!.Query);
    }
}
