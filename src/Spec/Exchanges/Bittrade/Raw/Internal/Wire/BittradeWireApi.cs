using System;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

internal sealed class BittradeWireApi : IBittradeWireApi
{
    public IBittradeWireMarketDataApi MarketData { get; }
    public IBittradeWireTradingApi Trading { get; }
    public IBittradeWireCommonApi Common { get; }

    public BittradeWireApi(IBittradeWireMarketDataApi marketData, IBittradeWireTradingApi trading)
        : this(marketData, trading, new BittradeWireCommonApiNotSupported())
    {
    }

    public BittradeWireApi(
        IBittradeWireMarketDataApi marketData,
        IBittradeWireTradingApi trading,
        IBittradeWireCommonApi common)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        Common = common ?? throw new ArgumentNullException(nameof(common));
    }
}
