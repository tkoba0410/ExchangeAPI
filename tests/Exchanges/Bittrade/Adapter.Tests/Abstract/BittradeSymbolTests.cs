using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class SymbolTests
{
    [Fact]
    public void TryParse_accepts_valid_symbol()
    {
        Assert.True(ExchangeSymbol.TryParse("btcjpy", out var symbol));
        Assert.Equal("btcjpy", symbol.ToString());
    }

    [Fact]
    public void TryParse_rejects_invalid_symbol()
    {
        Assert.False(ExchangeSymbol.TryParse("btc-jpy", out _));
        Assert.False(ExchangeSymbol.TryParse("", out _));
    }

    [Fact]
    public void Normalize_returns_normalized_value()
    {
        Assert.True(ExchangeSymbol.TryParse("BTC_JPY", out var normalized));
        Assert.Equal("btcjpy", normalized.ToString());
    }
}
