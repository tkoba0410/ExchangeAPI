using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Static;

internal static class BittradeMarketCatalog
{
    public const string BtcJpySymbol = "BTC/JPY";
    public const string BtcJpyProductCode = "btcjpy";
    private const string Spot = "Spot";

    public static IReadOnlyList<BittradeStaticMarketInfo> Markets { get; } =
        new[]
        {
            new BittradeStaticMarketInfo
            {
                Symbol = BtcJpySymbol,
                ProductCode = BtcJpyProductCode,
                Type = Spot,
                MinSize = 0.0001m,
                MinNotional = 1000m,
                PriceIncrement = 0.01m,
                SizeIncrement = 0.0001m,
                IsSupported = true,
                StatusNote = "online"
            }
        };
}
