using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Resolve;

internal static class ExchangeMarketCatalog
{
    public const string BtcJpySymbol = "BTC/JPY";
    public const string BtcJpyProductCode = "BTC_JPY";
    public const string XrpJpySymbol = "XRP/JPY";
    public const string XrpJpyProductCode = "XRP_JPY";
    public const string EthJpySymbol = "ETH/JPY";
    public const string EthJpyProductCode = "ETH_JPY";
    public const string XlmJpySymbol = "XLM/JPY";
    public const string XlmJpyProductCode = "XLM_JPY";
    public const string MonaJpySymbol = "MONA/JPY";
    public const string MonaJpyProductCode = "MONA_JPY";
    public const string ElfJpySymbol = "ELF/JPY";
    public const string ElfJpyProductCode = "ELF_JPY";
    public const string EthBtcSymbol = "ETH/BTC";
    public const string EthBtcProductCode = "ETH_BTC";
    public const string BchBtcSymbol = "BCH/BTC";
    public const string BchBtcProductCode = "BCH_BTC";
    public const string FxBtcJpySymbol = "FX/BTC/JPY";
    public const string FxBtcJpyProductCode = "FX_BTC_JPY";

    private const string Spot = "Spot";
    private const string Fx = "FX";

    public static string DefaultBoardProductCode => BtcJpyProductCode;

    public static IReadOnlyList<MarketCatalogEntry> Markets { get; } =
        new[]
        {
            CreateSpot(BtcJpySymbol, BtcJpyProductCode),
            CreateSpot(XrpJpySymbol, XrpJpyProductCode),
            CreateSpot(EthJpySymbol, EthJpyProductCode),
            CreateSpot(XlmJpySymbol, XlmJpyProductCode),
            CreateSpot(MonaJpySymbol, MonaJpyProductCode),
            CreateSpot(ElfJpySymbol, ElfJpyProductCode),
            CreateSpot(EthBtcSymbol, EthBtcProductCode),
            CreateSpot(BchBtcSymbol, BchBtcProductCode),
            CreateMarket(FxBtcJpySymbol, FxBtcJpyProductCode, Fx),
        };

    private static MarketCatalogEntry CreateSpot(string symbol, string productCode) =>
        CreateMarket(symbol, productCode, Spot);

    private static MarketCatalogEntry CreateMarket(string symbol, string productCode, string type) =>
        new(symbol, productCode, type);

    internal sealed record MarketCatalogEntry(string Symbol, string ProductCode, string Type);
}
