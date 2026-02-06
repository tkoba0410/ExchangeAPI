using System.Collections.Generic;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Utilities.Extensions;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts.Extensions;

public class ExchangeInfoExtensionsTests
{
    private readonly GetExchangeInfoResponse _info = new(
        Markets: new List<ExchangeMarketInfo>
        {
            new(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("BTC_JPY"), MarketType.ParseOrThrow("Spot"), MakerFeeRate: 0.001m, TakerFeeRate: 0.002m),
            new(Symbol.ParseOrThrow("ETH/JPY"), ProductCode.ParseOrThrow("ETH_JPY"), MarketType.ParseOrThrow("Spot"), MakerFeeRate: null, TakerFeeRate: null),
        },
        Features: null,
        RateLimits: null,
        Maintenance: null);

    [Fact]
    public void FindMarket_BySymbol_ShouldReturnMarket()
    {
        var market = _info.FindMarket(Symbol.ParseOrThrow("BTC/JPY"));
        Assert.NotNull(market);
        Assert.Equal("BTC_JPY", market!.ProductCode.Value);
    }

    [Fact]
    public void FindMarket_ByProductCode_ShouldReturnMarket()
    {
        var market = _info.FindMarket(Symbol.ParseOrThrow("unknown"), productCode: ProductCode.ParseOrThrow("ETH_JPY"));
        Assert.NotNull(market);
        Assert.Equal("ETH/JPY", market!.Symbol.Value);
    }

    [Fact]
    public void TryGetFeeRates_Found_ShouldReturnRates()
    {
        var info = new GetExchangeInfoResponse(
            Markets: new List<ExchangeMarketInfo>
            {
                new(Symbol.ParseOrThrow("BTC/JPY"), ProductCode.ParseOrThrow("BTC_JPY"), MarketType.ParseOrThrow("Spot"), MakerFeeRate: 0.001m, TakerFeeRate: 0.002m, FeeCurrency: CurrencyCode.Btc, FeeType: FeeType.Percentage),
            },
            Features: null,
            RateLimits: null,
            Maintenance: null);

        var found = info.TryGetFeeRates(Symbol.ParseOrThrow("BTC/JPY"), out var maker, out var taker, out var feeCurrency, out var feeType);

        Assert.True(found);
        Assert.Equal(0.001m, maker);
        Assert.Equal(0.002m, taker);
        Assert.Equal(CurrencyCode.Btc, feeCurrency);
        Assert.Equal(FeeType.Percentage, feeType);
    }

    [Fact]
    public void TryGetFeeRates_NotFound_ShouldReturnFalseAndNulls()
    {
        var found = _info.TryGetFeeRates(Symbol.ParseOrThrow("XRP/JPY"), out var maker, out var taker, out var feeCurrency, out var feeType);

        Assert.False(found);
        Assert.Null(maker);
        Assert.Null(taker);
        Assert.Null(feeCurrency);
        Assert.Null(feeType);
    }
}
