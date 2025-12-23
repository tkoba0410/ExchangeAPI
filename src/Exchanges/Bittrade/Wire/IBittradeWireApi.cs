using ExchangeApi.Exchanges.Bittrade.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Wire;

public interface IBittradeWireApi
{
    Public.IBittradeWireMarketDataApi MarketData { get; }
    IBittradeWireTradingApi Trading { get; }
}
