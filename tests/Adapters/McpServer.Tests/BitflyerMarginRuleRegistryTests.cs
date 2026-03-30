using ExchangeApi.Adapters.McpServer.Mapping;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class BitflyerMarginRuleRegistryTests
{
    [Fact]
    public void TryGet_ReturnsDocumentedBaseline()
    {
        var found = BitflyerMarginRuleRegistry.TryGet("FX_BTC_JPY", out var rule);

        Assert.True(found);
        Assert.NotNull(rule);
        Assert.Equal("FX_BTC_JPY", rule!.Symbol);
        Assert.Equal("0.001", rule.MinSize);
        Assert.Equal("0.00000001", rule.SizeStep);
        Assert.Equal("1", rule.PriceStep);
        Assert.Equal(MarginModelIds.RequireCollateralPriceTimesSizeDivCorporateLeverage, rule.RequireCollateralModel);
        Assert.Equal(MarginModelIds.CurrentKeepRateFloor, rule.MaintenanceModel);
        Assert.Equal("1.2", rule.MinimumKeepRate);
        Assert.Equal(MarginModelIds.OperationalPercentageFee, rule.FeeModel);
    }

    [Fact]
    public void Entries_ExposeOnlyTheCurrentBitflyerMarginSupportSet()
    {
        var symbols = BitflyerMarginRuleRegistry.Entries.Keys.OrderBy(x => x).ToArray();

        Assert.Equal(["FX_BTC_JPY"], symbols);
    }
}
