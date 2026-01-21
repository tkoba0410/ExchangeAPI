using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    IBittradeRawMarketDataApi MarketData { get; }
    IBittradeRawTradingApi Trading { get; }
    IBittradeRawExchangeInfoApi ExchangeInfo { get; }
    IBittradeRawAccountApi Account { get; }
}
