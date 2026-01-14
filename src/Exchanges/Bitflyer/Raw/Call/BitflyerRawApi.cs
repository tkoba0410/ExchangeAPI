using System;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Shared.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Call;

/// <summary>
/// bitFlyer の Mirror Raw API 入口。
/// </summary>
public sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public IBitflyerRawTradingApi Trading { get; }

    public BitflyerRawApi(IWireTransport wire)
        : this(
            new BitflyerRawMarketDataApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            new BitflyerRawTradingApi(wire))
    {
    }

    internal BitflyerRawApi(IBitflyerRawMarketDataApi marketData, IBitflyerRawTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }
}
