using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Adapters;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeSymbolMapperTests
{
    [Fact]
    public void ToProductCode_MapsToUpperUnderscore()
    {
        var symbol = new Symbol("BTC/JPY");

        var productCode = BittradeSymbolMapper.ToProductCode(symbol);

        Assert.Equal("BTC_JPY", productCode);
    }

    [Fact]
    public void ToApiSymbol_MapsToLowercaseConcatenated()
    {
        var symbol = new Symbol("BTC/JPY");

        var apiSymbol = BittradeSymbolMapper.ToApiSymbol(symbol);

        Assert.Equal("btcjpy", apiSymbol);
    }
}
