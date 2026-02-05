using System;
using ExchangeApi.Exchanges.Common.ExchangeInfo.Static;

namespace ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static;

internal static class BittradeStaticExchangeInfoLoader
{
    private const string ResourceName =
        "ExchangeApi.Exchanges.Bittrade.ExchangeInfo.Static.bittrade-exchange-info.json";
    private static readonly Lazy<BittradeStaticExchangeInfo> Cache = new(LoadInternal);

    public static BittradeStaticExchangeInfo Load() => Cache.Value;

    private static BittradeStaticExchangeInfo LoadInternal() =>
        StaticJsonLoader.Load<BittradeStaticExchangeInfo>(ResourceName);
}
