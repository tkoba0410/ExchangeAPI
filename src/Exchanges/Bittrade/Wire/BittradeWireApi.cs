using System;
using ExchangeApi.Exchanges.Bittrade.Wire.Public;

namespace ExchangeApi.Exchanges.Bittrade.Wire;

internal sealed class BittradeWireApi : IBittradeWireApi
{
    public IBittradeWireMarketDataApi MarketData { get; }

    public BittradeWireApi(IBittradeWireMarketDataApi marketData)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
    }
}
