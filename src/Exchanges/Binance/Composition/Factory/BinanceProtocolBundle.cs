using ExchangeApi.Exchanges.Binance.Protocol.Public.Api;

namespace ExchangeApi.Exchanges.Binance.Composition.Factory;

public sealed class BinanceProtocolBundle
{
    public required IBinancePublicProtocolApi Public { get; init; }
}
