using ExchangeApi.Exchanges.Bitflyer.Composition.Factory;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Composition.Tests;

public sealed class BitflyerClientFactoryTests
{
    [Fact]
    public void CreateProtocolClient_WithoutCredentials_HasOnlyPublic()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClient();

        Assert.NotNull(bundle.Public);
        Assert.Null(bundle.Private);
    }

    [Fact]
    public void CreateProtocolClient_WithCredentials_HasPrivate()
    {
        var bundle = BitflyerClientFactory.CreateProtocolClient(new BitflyerClientOptions
        {
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = "key",
                ApiSecret = "secret",
            },
        });

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Private);
    }

    [Fact]
    public void CreateNativeClient_WithCredentials_WiresProtocolAndNative()
    {
        var bundle = BitflyerClientFactory.CreateNativeClient(new BitflyerClientOptions
        {
            Credentials = new BitflyerApiCredentials
            {
                ApiKey = "key",
                ApiSecret = "secret",
            },
        });

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Private);
        Assert.NotNull(bundle.Protocol.Public);
        Assert.NotNull(bundle.Protocol.Private);
    }
}
