using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

#pragma warning disable CS0618
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

internal interface IBitflyerWireApi
{
    IBitflyerWireMarketDataApi MarketData { get; }
    IBitflyerWireTradingApi Trading { get; }
    IBitflyerWireAccountApi Account { get; }
    IBitflyerWireExchangeInfoApi ExchangeInfo { get; }
}
#pragma warning restore CS0618
