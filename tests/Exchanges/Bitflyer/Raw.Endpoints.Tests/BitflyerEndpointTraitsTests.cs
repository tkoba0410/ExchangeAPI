using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using Xunit;

namespace Exchange.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerEndpointTraitsTests
{
    [Fact]
    public void RequiresAuth_ReturnsTrue_ForPrivateEndpoint()
    {
        Assert.True(EndpointTraits.RequiresAuth(EndpointIds.GetBalance));
    }

    [Fact]
    public void RequiresAuth_ReturnsFalse_ForPublicEndpoint()
    {
        Assert.False(EndpointTraits.RequiresAuth(EndpointIds.GetTicker));
    }
}
