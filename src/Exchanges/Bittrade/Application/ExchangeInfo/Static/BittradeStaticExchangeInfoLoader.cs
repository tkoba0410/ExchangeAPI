using System;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static;

internal static class BittradeStaticExchangeInfoLoader
{
    private const string ResourceName =
        "ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static.bittrade-exchange-info.json";
    private static readonly Lazy<BittradeStaticExchangeInfo> Cache = new(LoadInternal);

    public static BittradeStaticExchangeInfo Load() => Cache.Value;

    private static BittradeStaticExchangeInfo LoadInternal()
    {
        var info = StaticJsonLoader.Load<BittradeStaticExchangeInfo>(ResourceName);
        return new BittradeStaticExchangeInfo
        {
            Markets = BittradeMarketCatalog.Markets,
            Features = info.Features,
            RateLimits = info.RateLimits,
            Maintenance = info.Maintenance
        };
    }
}
