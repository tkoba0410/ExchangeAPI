using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Adapters;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerSymbolMapperTests
{
    [Fact]
    public void ToProductCode_MapsCanonicalSymbol()
    {
        var symbol = new Symbol("BTC/JPY");
        var code = BitflyerSymbolMapper.ToProductCode(symbol);

        Assert.Equal(ProductCode.BtcJpy, code);
    }

    [Fact]
    public void FromProductCode_MapsToCanonicalSymbol()
    {
        var symbol = BitflyerSymbolMapper.FromProductCode("BTC_JPY");

        Assert.Equal(new Symbol("BTC/JPY"), symbol);
    }
}
