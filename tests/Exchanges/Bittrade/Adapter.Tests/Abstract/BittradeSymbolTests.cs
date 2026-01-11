using System;
using ExchangeApi.Exchanges.Bittrade.Normalize.Types;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeSymbolTests
{
    [Fact]
    public void TryParse_accepts_valid_symbol()
    {
        Assert.True(BittradeSymbol.TryParse("btcjpy", out var symbol));
        Assert.Equal("btcjpy", symbol.ToString());
    }

    [Fact]
    public void TryParse_rejects_invalid_symbol()
    {
        Assert.False(BittradeSymbol.TryParse("BTC_JPY", out _));
        Assert.False(BittradeSymbol.TryParse("", out _));
    }

    [Fact]
    public void ParseOrThrow_throws_with_clear_message()
    {
        var ex = Assert.Throws<ArgumentException>(() => BittradeSymbol.ParseOrThrow("BTC_JPY"));
        Assert.Contains("Bittrade symbol is invalid", ex.Message, StringComparison.Ordinal);
    }
}
