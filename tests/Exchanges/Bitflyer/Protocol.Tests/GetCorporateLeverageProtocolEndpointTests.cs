using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetCorporateLeverage;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetCorporateLeverageProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_UsesPublicGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCorporateLeverageProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getcorporateleverage", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Null(transport.LastRequest.Query);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }
}
