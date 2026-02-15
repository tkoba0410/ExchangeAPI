using System;
using ExchangeApi.Exchanges.Common.Application.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Static;

internal static class BitflyerStaticExchangeInfoLoader
{
    private const string ResourceName =
        "ExchangeApi.Exchanges.Bitflyer.Application.ExchangeInfo.Static.bitflyer-exchange-info.json";
    private static readonly Lazy<BitflyerStaticExchangeInfo> Cache = new(LoadInternal);

    public static BitflyerStaticExchangeInfo Load() => Cache.Value;

    private static BitflyerStaticExchangeInfo LoadInternal()
    {
        var info = StaticJsonLoader.Load<BitflyerStaticExchangeInfo>(ResourceName);
        return new BitflyerStaticExchangeInfo
        {
            Markets = BitflyerMarketCatalog.Markets,
            Features = info.Features,
            RateLimits = info.RateLimits,
            Maintenance = info.Maintenance,
            FeeSchedule = info.FeeSchedule
        };
    }
}
