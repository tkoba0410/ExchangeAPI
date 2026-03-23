using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class GetTickerProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_OmitsQuery_WhenProductCodeIsNull()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetTickerProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(null);

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Null(transport.LastRequest!.Query);
        Assert.Equal("/v1/getticker", transport.LastRequest.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }

    [Fact]
    public async Task SendAsync_SetsQuery_WhenProductCodeIsPresent()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetTickerProtocolEndpoint(transport);

        await endpoint.SendAsync("BTC_JPY");

        Assert.NotNull(transport.LastRequest);
        Assert.NotNull(transport.LastRequest!.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
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

        var endpoint = new GetTickerProtocolEndpoint(transport);
        var call = await endpoint.SendAsync(null);

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, call.Error!.Kind);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }
}
