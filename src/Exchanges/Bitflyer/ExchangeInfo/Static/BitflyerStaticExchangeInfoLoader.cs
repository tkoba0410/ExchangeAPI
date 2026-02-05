using System;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Static;

internal static class BitflyerStaticExchangeInfoLoader
{
    private const string ResourceName =
        "ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Static.bitflyer-exchange-info.json";
    private static readonly Lazy<BitflyerStaticExchangeInfo> Cache = new(LoadInternal);

    public static BitflyerStaticExchangeInfo Load() => Cache.Value;

    private static BitflyerStaticExchangeInfo LoadInternal() =>
        StaticJsonLoader.Load<BitflyerStaticExchangeInfo>(ResourceName);
}
