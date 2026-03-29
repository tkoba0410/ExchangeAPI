using ExchangeApi.Adapters.McpServer.Mapping;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class BitflyerMarketRuleRegistryTests
{
    [Theory]
    [InlineData("BTC_JPY", "0.001", "0.00000001", "1")]
    [InlineData("FX_BTC_JPY", "0.001", "0.00000001", "1")]
    public void TryGet_ReturnsDocumentedBaseline(
        string symbol,
        string expectedMinSize,
        string expectedSizeStep,
        string expectedPriceStep)
    {
        var found = BitflyerMarketRuleRegistry.TryGet(symbol, out var rule);

        Assert.True(found);
        Assert.NotNull(rule);
        Assert.Equal(symbol, rule.Symbol);
        Assert.Equal(expectedMinSize, rule.MinSize);
        Assert.Equal(expectedSizeStep, rule.SizeStep);
        Assert.Equal(expectedPriceStep, rule.PriceStep);
    }

    [Fact]
    public void Entries_ExposeOnlyTheCurrentBitflyerV1SupportSet()
    {
        var symbols = BitflyerMarketRuleRegistry.Entries.Keys.OrderBy(x => x).ToArray();

        Assert.Equal(["BTC_JPY", "FX_BTC_JPY"], symbols);
    }
}
