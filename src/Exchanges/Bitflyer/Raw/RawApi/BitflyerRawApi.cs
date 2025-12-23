using System;
using ExchangeApi.Core.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>
/// bitFlyer の Mirror Raw API 入口。
/// </summary>
public sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }

    public BitflyerRawApi(IRestClient restClient)
        : this(new BitflyerRawMarketDataApi(restClient ?? throw new ArgumentNullException(nameof(restClient))))
    {
    }

    internal BitflyerRawApi(IBitflyerRawMarketDataApi marketData)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
    }
}
