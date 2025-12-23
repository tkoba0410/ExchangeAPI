namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    IBittradeRawMarketDataApi MarketData { get; }
}
