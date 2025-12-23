namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer Raw API のバンドル。
/// </summary>
public interface IBitflyerRawApi
{
    IBitflyerRawMarketDataApi MarketData { get; }
}
