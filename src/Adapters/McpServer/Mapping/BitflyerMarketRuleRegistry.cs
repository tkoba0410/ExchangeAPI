namespace ExchangeApi.Adapters.McpServer.Mapping;

public static class BitflyerMarketRuleRegistry
{
    private static readonly IReadOnlyDictionary<string, BitflyerMarketRule> Rules =
        new Dictionary<string, BitflyerMarketRule>(StringComparer.Ordinal)
        {
            ["BTC_JPY"] = new(
                Symbol: "BTC_JPY",
                MinSize: "0.001",
                SizeStep: "0.00000001",
                PriceStep: "1",
                SourceNote: "minSize and sizeStep are documented; priceStep is adapter-owned inference"),
            ["FX_BTC_JPY"] = new(
                Symbol: "FX_BTC_JPY",
                MinSize: "0.001",
                SizeStep: "0.00000001",
                PriceStep: "1",
                SourceNote: "minSize reflects the post-2024-10-21 CFD change; priceStep is adapter-owned inference"),
        };

    public static IReadOnlyDictionary<string, BitflyerMarketRule> Entries => Rules;

    public static bool TryGet(string symbol, out BitflyerMarketRule? rule)
    {
        return Rules.TryGetValue(symbol, out rule);
    }
}
