using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

internal interface IBittradeWireApi
{
    Public.IBittradeWireMarketDataApi MarketData { get; }
    IBittradeWireTradingApi Trading { get; }
}
