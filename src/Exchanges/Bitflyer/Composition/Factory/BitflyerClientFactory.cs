using ExchangeApi.Exchanges.Bitflyer.Composition.Bootstrap;
using ExchangeApi.Exchanges.Bitflyer.Composition.Options;

namespace ExchangeApi.Exchanges.Bitflyer.Composition.Factory;

public static class BitflyerClientFactory
{
    public static BitflyerProtocolBundle CreateProtocolClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateProtocolBundle(options);
    }

    public static BitflyerProtocolBundle CreateProtocolClient(HttpClient httpClient, BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateProtocolBundle(httpClient, options);
    }

    public static BitflyerNativeBundle CreateNativeClient(BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateNativeBundle(options);
    }

    public static BitflyerNativeBundle CreateNativeClient(HttpClient httpClient, BitflyerClientOptions? options = null)
    {
        return BitflyerBootstrap.CreateNativeBundle(httpClient, options);
    }
}
