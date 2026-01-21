using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Raw API のバンドル。
/// </summary>
public interface IBitflyerRawApi
{
    IBitflyerRawMarketDataApi MarketData { get; }
    IBitflyerRawTradingApi Trading { get; }
    IBitflyerRawAccountApi Account { get; }
}
