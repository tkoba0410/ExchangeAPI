using System;
using ExchangeApi.Exchanges.Bittrade.Wire.Public;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Wire;

internal sealed class BittradeWireApi : IBittradeWireApi
{
    public IBittradeWireMarketDataApi MarketData { get; }
    public IBittradeWireTradingApi Trading { get; }

    public BittradeWireApi(IBittradeWireMarketDataApi marketData, IBittradeWireTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }
}
