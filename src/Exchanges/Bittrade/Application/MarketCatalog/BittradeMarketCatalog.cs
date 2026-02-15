using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bittrade.Application.MarketCatalog;

internal static class BittradeMarketCatalog
{
    public const string BtcJpySymbol = "BTC/JPY";
    public const string BtcJpyProductCode = "btcjpy";
    private const string Spot = "Spot";

    public static IReadOnlyList<MarketCatalogEntry> Markets { get; } =
        new[]
        {
            new MarketCatalogEntry(
                BtcJpySymbol,
                BtcJpyProductCode,
                Spot,
                MinSize: 0.0001m,
                MinNotional: 1000m,
                PriceIncrement: 0.01m,
                SizeIncrement: 0.0001m)
        };

    internal sealed record MarketCatalogEntry(
        string Symbol,
        string ProductCode,
        string Type,
        decimal? MinSize = null,
        decimal? MinNotional = null,
        decimal? PriceIncrement = null,
        decimal? SizeIncrement = null);
}
