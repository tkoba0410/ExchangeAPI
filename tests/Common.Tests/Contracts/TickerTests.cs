using System;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using Xunit;

namespace Common.Tests.Contracts;

public class TickerTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var lastPrice = 5_000_500m;
        var timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero); // UTC

        // Act
        var ticker = new Ticker(
            Exchange: ExchangeCode.None,
            Symbol: Symbol.BtcJpy,
            LastTradedPrice: lastPrice,
            Timestamp: timestamp);

        // Assert
        Assert.Equal(ExchangeCode.None, ticker.Exchange);
        Assert.Equal(Symbol.BtcJpy, ticker.Symbol);
        Assert.Equal(lastPrice, ticker.LastTradedPrice);
        Assert.Equal(timestamp, ticker.Timestamp);
    }

    [Fact]
    public void WithExpression_CreatesModifiedInstance()
    {
        // Arrange
        var original = new Ticker(
            Exchange: ExchangeCode.None,
            Symbol: Symbol.BtcJpy,
            LastTradedPrice: 5_000_500m,
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { LastTradedPrice = 4_999_000m };

        // Assert
        Assert.Equal(5_000_500m, original.LastTradedPrice);
        Assert.Equal(4_999_000m, modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }

    [Fact]
    public void Ticker_IsImmutable()
    {
        // Arrange
        var original = new Ticker(
            Exchange: ExchangeCode.None,
            Symbol: Symbol.BtcJpy,
            LastTradedPrice: 5_000_500m,
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { LastTradedPrice = 4_999_000m };

        // Assert
        Assert.Equal(5_000_500m, original.LastTradedPrice);
        Assert.Equal(4_999_000m, modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }
}
