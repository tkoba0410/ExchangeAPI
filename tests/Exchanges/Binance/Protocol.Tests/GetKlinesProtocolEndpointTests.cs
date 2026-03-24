using ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Binance.Protocol.Public.Endpoints.GetKlines;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Tests.Exchanges.Binance.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Binance.Protocol.Tests;

public sealed class GetKlinesProtocolEndpointTests
{
    [Fact]
    public async Task SendAsync_OmitsOptionalQuery_WhenValuesAreNull()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetKlinesProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTCJPY", "1h");

        Assert.True(call.IsSuccess);
        Assert.NotNull(transport.LastRequest);
        Assert.Equal("/api/v3/klines", transport.LastRequest!.Path);
        Assert.Equal("GET", transport.LastRequest.Method);
        Assert.Equal("BTCJPY", transport.LastRequest.Query!["symbol"]);
        Assert.Equal("1h", transport.LastRequest.Query["interval"]);
        Assert.DoesNotContain("startTime", transport.LastRequest.Query.Keys);
        Assert.DoesNotContain("endTime", transport.LastRequest.Query.Keys);
        Assert.DoesNotContain("timeZone", transport.LastRequest.Query.Keys);
        Assert.DoesNotContain("limit", transport.LastRequest.Query.Keys);
        Assert.Equal(ProtocolTransportAuthMode.None, transport.LastAuthMode);
        Assert.Equal(CallComponents.PublicEndpointModule, call.Meta.Component);
    }

    [Fact]
    public async Task SendAsync_IncludesOptionalQuery_WhenValuesArePresent()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetKlinesProtocolEndpoint(transport);

        await endpoint.SendAsync("BTCJPY", "1d", 1L, 2L, "+09:00", 1000);

        Assert.NotNull(transport.LastRequest);
        Assert.Equal("1", transport.LastRequest!.Query!["startTime"]);
        Assert.Equal("2", transport.LastRequest.Query["endTime"]);
        Assert.Equal("+09:00", transport.LastRequest.Query["timeZone"]);
        Assert.Equal("1000", transport.LastRequest.Query["limit"]);
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

        var endpoint = new GetKlinesProtocolEndpoint(transport);
        var call = await endpoint.SendAsync("BTCJPY", "1h");

        Assert.False(call.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, call.Error!.Kind);
    }
}
