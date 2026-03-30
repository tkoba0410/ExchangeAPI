using ExchangeApi.Adapters.McpServer.Mapping;

namespace ExchangeApi.Adapters.McpServer.Tests;

public sealed class BinanceKlineSymbolSetTests
{
    [Fact]
    public void Entries_ContainTheDocumentedSupportSet()
    {
        var symbols = BinanceKlineSymbolSet.Entries.OrderBy(x => x).ToArray();

        Assert.Equal(["BNBJPY", "BTCJPY", "BTCUSDT", "ETHJPY", "ETHUSDT", "SOLUSDT", "XRPJPY", "XRPUSDT"], symbols);
    }

    [Theory]
    [InlineData("BTCJPY")]
    [InlineData("ETHJPY")]
    [InlineData("XRPJPY")]
    [InlineData("BNBJPY")]
    [InlineData("BTCUSDT")]
    [InlineData("ETHUSDT")]
    [InlineData("SOLUSDT")]
    [InlineData("XRPUSDT")]
    public void Contains_ReturnsTrueForSupportedSymbol(string symbol)
    {
        Assert.True(BinanceKlineSymbolSet.Contains(symbol));
    }

    [Fact]
    public void Contains_ReturnsFalseForUnsupportedSymbol()
    {
        Assert.False(BinanceKlineSymbolSet.Contains("SOLJPY"));
    }
}
