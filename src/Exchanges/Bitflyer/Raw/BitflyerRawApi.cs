using System;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer の Mirror Raw API 入口。
/// </summary>
public sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public IBitflyerRawTradingApi Trading { get; }
    public IBitflyerRawAccountApi Account { get; }

    public BitflyerRawApi(IWireTransport wire)
        : this(
            new BitflyerRawMarketDataApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            new BitflyerRawTradingApi(wire),
            new BitflyerRawAccountApi(wire))
    {
    }

    internal BitflyerRawApi(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerRawTradingApi trading,
        IBitflyerRawAccountApi account)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        Account = account ?? throw new ArgumentNullException(nameof(account));
    }
}
