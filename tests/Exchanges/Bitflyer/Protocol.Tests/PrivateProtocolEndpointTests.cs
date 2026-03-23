using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class PrivateProtocolEndpointTests
{
    [Fact]
    public async Task GetBalance_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBalanceProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getbalance", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task SendChildOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new SendChildOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/sendchildorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task CancelChildOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new CancelChildOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/cancelchildorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }
}
