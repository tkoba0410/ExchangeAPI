using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Wire;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Normalize;

internal sealed class BittradeNormalizeBundle
{
    public IBittradeNormalizedMarketDataApi MarketData { get; }
    public IBittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public IBittradeNormalizedAccountApi? Account { get; }
    public IBittradeWireTradingApi Trading { get; }
    public object RawBundle { get; }
    public BittradeWireApi WireBundle { get; }

    public BittradeNormalizeBundle(
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi? account,
        IBittradeWireTradingApi trading,
        object rawBundle,
        BittradeWireApi wireBundle)
    {
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account;
        Trading = trading;
        RawBundle = rawBundle;
        WireBundle = wireBundle;
    }
}
