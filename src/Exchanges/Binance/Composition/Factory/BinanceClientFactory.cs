using ExchangeApi.Exchanges.Binance.Composition.Bootstrap;
using ExchangeApi.Exchanges.Binance.Composition.Options;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

public static class BinanceClientFactory
{
    public static BinanceProtocolBundle CreateProtocolClient(BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateProtocolBundle(options);
    }

    public static BinanceNativeBundle CreateNativeClient(BinanceClientOptions? options = null)
    {
        return BinanceBootstrap.CreateNativeBundle(options);
    }
}
