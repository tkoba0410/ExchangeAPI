using ExchangeApi.Exchanges.Binance.Vocabulary;

namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BinanceKlineSymbolSet
{
    private static readonly HashSet<string> Symbols =
    [
        BinanceSymbols.BtcJpy,
        BinanceSymbols.EthJpy,
        BinanceSymbols.XrpJpy,
        BinanceSymbols.BnbJpy,
        BinanceSymbols.BtcUsdt,
        BinanceSymbols.EthUsdt,
        BinanceSymbols.SolUsdt,
        BinanceSymbols.XrpUsdt,
    ];

    public static IReadOnlyCollection<string> Entries => Symbols;

    public static bool Contains(string symbol)
    {
        return Symbols.Contains(symbol);
    }
}
