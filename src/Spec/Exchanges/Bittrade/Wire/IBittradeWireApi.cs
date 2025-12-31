using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

public interface IBittradeWireApi
{
    Public.IBittradeWireMarketDataApi MarketData { get; }
    IBittradeWireTradingApi Trading { get; }
    Private.IBittradeWireAccountApi Account { get; }
    Public.IBittradeWireCommonApi Common { get; }
}
