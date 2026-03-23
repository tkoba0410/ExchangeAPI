using ExchangeApi.Exchanges.Bitflyer.Composition.Bootstrap;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

public static class BitflyerClientFactory
{
    public static BitflyerProtocolBundle CreateProtocolClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateProtocolBundle(options);
    }

    public static BitflyerNativeBundle CreateNativeClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateNativeBundle(options);
    }
}
