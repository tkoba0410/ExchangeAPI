using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    IBittradeRawMarketDataApi MarketData { get; }
    IBittradeRawTradingApi Trading { get; }
    IBittradeRawExchangeInfoApi ExchangeInfo { get; }
    IBittradeRawAccountApi Account { get; }
}
