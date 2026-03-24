using ExchangeApi.Exchanges.Binance.Native.Public.Api;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

public sealed class BinanceNativeBundle
{
    public required IBinancePublicNativeApi Public { get; init; }
    public required BinanceProtocolBundle Protocol { get; init; }
}
