using ExchangeApi.Adapters.McpServer.Mapping;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class BitflyerMarketRuleRegistryTests
{
    [Theory]
    [InlineData("BTC_JPY", "0.001", "0.00000001", "1", MarketRuleSourceKinds.OfficialDocumented, "https://bitflyer.com/ja-jp/s/commission", MarketRuleSourceKinds.OfficialDocumented, "https://bitflyer.com/ja-jp/s/commission", MarketRuleSourceKinds.AdapterInferred, "adapter://bitflyer-jpy-price-step.v1")]
    [InlineData("FX_BTC_JPY", "0.001", "0.00000001", "1", MarketRuleSourceKinds.OfficialDocumented, "https://bitflyer.com/pub/20241015-bitFlyerCryptoCFD-Minimum-Order-Change-en.pdf", MarketRuleSourceKinds.OfficialDocumented, "https://bitflyer.com/ja-jp/s/commission", MarketRuleSourceKinds.AdapterInferred, "adapter://bitflyer-jpy-price-step.v1")]
    public void TryGet_ReturnsDocumentedBaseline(
        string symbol,
        string expectedMinSize,
        string expectedSizeStep,
        string expectedPriceStep,
        string expectedMinSizeSourceKind,
        string expectedMinSizeSourceRef,
        string expectedSizeStepSourceKind,
        string expectedSizeStepSourceRef,
        string expectedPriceStepSourceKind,
        string expectedPriceStepSourceRef)
    {
        var found = BitflyerMarketRuleRegistry.TryGet(symbol, out var rule);

        Assert.True(found);
        Assert.NotNull(rule);
        Assert.Equal(symbol, rule.Symbol);
        Assert.Equal(expectedMinSize, rule.MinSize);
        Assert.Equal(expectedSizeStep, rule.SizeStep);
        Assert.Equal(expectedPriceStep, rule.PriceStep);
        Assert.Equal(expectedMinSizeSourceKind, rule.MinSizeSourceKind);
        Assert.Equal(expectedMinSizeSourceRef, rule.MinSizeSourceRef);
        Assert.Equal(expectedSizeStepSourceKind, rule.SizeStepSourceKind);
        Assert.Equal(expectedSizeStepSourceRef, rule.SizeStepSourceRef);
        Assert.Equal(expectedPriceStepSourceKind, rule.PriceStepSourceKind);
        Assert.Equal(expectedPriceStepSourceRef, rule.PriceStepSourceRef);
    }

    [Fact]
    public void Entries_ExposeOnlyTheCurrentBitflyerV1SupportSet()
    {
        var symbols = BitflyerMarketRuleRegistry.Entries.Keys.OrderBy(x => x).ToArray();

        Assert.Equal(["BTC_JPY", "FX_BTC_JPY"], symbols);
    }
}
