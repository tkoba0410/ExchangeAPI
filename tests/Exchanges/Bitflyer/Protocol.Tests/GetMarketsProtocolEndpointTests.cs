using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetMarkets;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetMarketsProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_UsesPublicGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetMarketsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/v1/getmarkets", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }

    [Fact]
    public async Task SendAsync_ReturnsFailure_WhenTransportFails()
    {
        var transport = new FakeProtocolTransport
        {
            Handler = (_, _) => new ProtocolTransportResult
            {
                IsSuccess = false,
                Error = new CallError
                {
                    Kind = CallErrorKinds.Transport,
                    Message = "boom",
                },
            },
        };

        var endpoint = new GetMarketsProtocolEndpoint(transport);
        var call = await endpoint.SendAsync();

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, call.Error!.Kind);
    }
}
