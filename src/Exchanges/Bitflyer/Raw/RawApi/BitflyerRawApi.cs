using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer の Mirror Raw API 入口。
/// </summary>
public sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public IBitflyerRawTradingApi Trading { get; }

    public BitflyerRawApi(IRestClient restClient)
        : this(
            new BitflyerRawMarketDataApi(restClient ?? throw new ArgumentNullException(nameof(restClient))),
            new Private.BitflyerRawTradingApi(restClient))
    {
    }

    internal BitflyerRawApi(IBitflyerRawMarketDataApi marketData, IBitflyerRawTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }
}
