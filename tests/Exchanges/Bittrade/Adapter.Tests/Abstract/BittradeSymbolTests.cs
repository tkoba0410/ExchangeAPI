using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Types;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

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
        Assert.False(BittradeSymbol.TryParse("btc-jpy", out _));
        Assert.False(BittradeSymbol.TryParse("", out _));
    }

    [Fact]
    public void ParseOrThrow_throws_with_clear_message()
    {
        var ex = Assert.Throws<ArgumentException>(() => BittradeSymbol.ParseOrThrow("btc-jpy"));
        Assert.Contains("Bittrade symbol is invalid", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Normalize_returns_value_accepted_by_ParseOrThrow()
    {
        var normalized = BittradeSymbol.Normalize("BTC_JPY");
        var parsed = BittradeSymbol.ParseOrThrow(normalized);

        Assert.Equal("btcjpy", parsed.ToString());
    }
}
