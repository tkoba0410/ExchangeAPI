using ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Raw API のバンドル。
/// </summary>
public interface IBitflyerRawApi
{
    IBitflyerRawMarketDataApi MarketData { get; }
    IBitflyerRawTradingApi Trading { get; }
}
