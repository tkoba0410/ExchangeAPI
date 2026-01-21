using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using Xunit;

namespace Exchange.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerEndpointTraitsTests
{
    [Fact]
    public void RequiresAuth_ReturnsTrue_ForPrivateEndpoint()
    {
        Assert.True(BitflyerEndpointTraits.RequiresAuth(BitflyerEndpointIds.GetBalance));
    }

    [Fact]
    public void RequiresAuth_ReturnsFalse_ForPublicEndpoint()
    {
        Assert.False(BitflyerEndpointTraits.RequiresAuth(BitflyerEndpointIds.GetTicker));
    }
}
