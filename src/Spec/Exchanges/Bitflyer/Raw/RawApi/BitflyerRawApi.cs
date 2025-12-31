using System;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer の Mirror Raw API 入口。
/// </summary>
public sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public IBitflyerRawTradingApi Trading { get; }

    public BitflyerRawApi(IBitflyerWireApi wire)
        : this(
            new BitflyerRawMarketDataApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            new Private.BitflyerRawTradingApi(wire))
    {
    }

    internal BitflyerRawApi(IBitflyerRawMarketDataApi marketData, IBitflyerRawTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }
}
