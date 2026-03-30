namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BinanceKlineSymbolSet
{
    private static readonly HashSet<string> Symbols =
    [
        "BTCJPY",
        "ETHJPY",
        "XRPJPY",
        "BNBJPY",
        "BTCUSDT",
        "ETHUSDT",
        "SOLUSDT",
        "XRPUSDT",
    ];

    public static IReadOnlyCollection<string> Entries => Symbols;

    public static bool Contains(string symbol)
    {
        return Symbols.Contains(symbol);
    }
}
