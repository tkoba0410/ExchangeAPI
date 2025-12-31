using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

public interface IBitflyerWireApi
{
    IBitflyerWireMarketDataApi MarketData { get; }
    IBitflyerWireTradingApi Trading { get; }
    IBitflyerWireAccountApi Account { get; }
    IBitflyerWireExchangeInfoApi ExchangeInfo { get; }
}
