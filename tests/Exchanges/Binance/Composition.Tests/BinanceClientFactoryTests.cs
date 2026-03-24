using ExchangeApi.Exchanges.Binance.Composition.Factory;

namespace ExchangeApi.Tests.Exchanges.Binance.Composition.Tests;

public sealed class BinanceClientFactoryTests
{
    [Fact]
    public void CreateProtocolClient_HasPublic()
    {
        var bundle = BinanceClientFactory.CreateProtocolClient();

        Assert.NotNull(bundle.Public);
    }

    [Fact]
    public void CreateNativeClient_WiresProtocolAndNative()
    {
        var bundle = BinanceClientFactory.CreateNativeClient();

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Protocol);
        Assert.NotNull(bundle.Protocol.Public);
    }
}
