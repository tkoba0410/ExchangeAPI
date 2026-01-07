using System;
using ExchangeApi.Common.Types;
using Xunit;

namespace Common.Tests;

public sealed class SymbolSafetyTests
{
    [Fact]
    public void DefaultSymbol_IsEmpty()
    {
        var symbol = default(Symbol);
        Assert.True(symbol.IsEmpty);
        Assert.Equal(string.Empty, symbol.ToString());
    }

    [Fact]
    public void Parse_Empty_Throws()
    {
        Assert.Throws<ArgumentException>(() => Symbol.Parse(" "));
    }

    [Fact]
    public void TryParse_Empty_ReturnsFalse()
    {
        var ok = Symbol.TryParse(" ", out var symbol);
        Assert.False(ok);
        Assert.True(symbol.IsEmpty);
    }
}
