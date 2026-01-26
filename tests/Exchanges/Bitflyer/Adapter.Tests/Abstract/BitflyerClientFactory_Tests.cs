using ExchangeApi.Transport.Policy;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Factory;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

    public class BitflyerClientFactory_Tests
    {
    [Fact]
    public void Create_WithOptions_Succeeds()
    {
        var options = new BitflyerClientOptions
        {
            PolicyOptions = new HttpPolicyOptions { RequestsPerSecond = 10 }
        };

        var client = BitflyerClientFactory.Create("key-1", "secret-1", options);

        Assert.NotNull(client);
    }
}
