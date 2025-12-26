using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;

namespace ExchangeApi.Exchanges.Bitflyer.Wire;

public interface IBitflyerWireApi
{
    IBitflyerWireMarketDataApi MarketData { get; }
    IBitflyerWireTradingApi Trading { get; }
    IBitflyerWireAccountApi Account { get; }
    IBitflyerWireExchangeInfoApi ExchangeInfo { get; }
}
