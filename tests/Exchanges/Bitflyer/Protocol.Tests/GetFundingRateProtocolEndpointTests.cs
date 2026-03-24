using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetFundingRate;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetFundingRateProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_UsesPublicGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetFundingRateProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("FX_BTC_JPY");

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getfundingrate", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Equal("FX_BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }
}
