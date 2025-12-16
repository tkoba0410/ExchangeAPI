using System.Collections.Generic;
using Common.Dtos;
using Common.Enums;
using Common.Extensions;
using Xunit;

namespace Common.Tests.Contracts.Extensions;

public class ExchangeInfoExtensionsTests
{
    private readonly ExchangeInfo _info = new(
        Markets: new List<ExchangeMarketInfo>
        {
            new("BTC/JPY", "BTC_JPY", "Spot", MakerFeeRate: 0.001m, TakerFeeRate: 0.002m),
            new("ETH/JPY", "ETH_JPY", "Spot", MakerFeeRate: null, TakerFeeRate: null),
        },
        Features: null,
        RateLimits: null,
        Maintenance: null);

    [Fact]
    public void FindMarket_BySymbol_ShouldReturnMarket()
    {
        var market = _info.FindMarket("BTC/JPY");
        Assert.NotNull(market);
        Assert.Equal("BTC_JPY", market!.ProductCode);
    }

    [Fact]
    public void FindMarket_ByProductCode_ShouldReturnMarket()
    {
        var market = _info.FindMarket("unknown", productCode: "ETH_JPY");
        Assert.NotNull(market);
        Assert.Equal("ETH/JPY", market!.Symbol);
    }

    [Fact]
    public void TryGetFeeRates_Found_ShouldReturnRates()
    {
        var info = new ExchangeInfo(
            Markets: new List<ExchangeMarketInfo>
            {
                new("BTC/JPY", "BTC_JPY", "Spot", MakerFeeRate: 0.001m, TakerFeeRate: 0.002m, FeeCurrency: "BTC", FeeType: FeeType.Percentage),
            },
            Features: null,
            RateLimits: null,
            Maintenance: null);

        var found = info.TryGetFeeRates("BTC/JPY", out var maker, out var taker, out var feeCurrency, out var feeType);

        Assert.True(found);
        Assert.Equal(0.001m, maker);
        Assert.Equal(0.002m, taker);
        Assert.Equal("BTC", feeCurrency);
        Assert.Equal(FeeType.Percentage, feeType);
    }

    [Fact]
    public void TryGetFeeRates_NotFound_ShouldReturnFalseAndNulls()
    {
        var found = _info.TryGetFeeRates("XRP/JPY", out var maker, out var taker, out var feeCurrency, out var feeType);

        Assert.False(found);
        Assert.Null(maker);
        Assert.Null(taker);
        Assert.Null(feeCurrency);
        Assert.Null(feeType);
    }
}
