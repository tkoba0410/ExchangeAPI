using System;
using ExchangeApi.Contracts.Dtos;
using Xunit;

namespace ExchangeApi.Contracts.Tests;

public class TickerTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var symbol = "BTC/JPY";
        var bestBid = 5_000_000m;
        var bestAsk = 5_001_000m;
        var lastPrice = 5_000_500m;
        var timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero); // UTC

        // Act
        var ticker = new Ticker(
            symbol,
            bestBid,
            bestAsk,
            lastPrice,
            timestamp);

        // Assert
        Assert.Equal(symbol, ticker.Symbol);
        Assert.Equal(bestBid, ticker.BestBid);
        Assert.Equal(bestAsk, ticker.BestAsk);
        Assert.Equal(lastPrice, ticker.LastTradedPrice);
        Assert.Equal(timestamp, ticker.Timestamp);
    }

    [Fact]
    public void WithExpression_CreatesModifiedInstance()
    {
        // Arrange
        var original = new Ticker(
            "BTC/JPY",
            5_000_000m,
            5_001_000m,
            5_000_500m,
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { BestBid = 4_999_000m };

        // Assert
        Assert.Equal(5_000_000m, original.BestBid);
        Assert.Equal(4_999_000m, modified.BestBid);
        Assert.NotSame(original, modified);
    }

    [Fact]
    public void Ticker_IsImmutable()
    {
        // Arrange
        var original = new Ticker(
            "BTC/JPY",
            5_000_000m,
            5_001_000m,
            5_000_500m,
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { BestBid = 4_999_000m };

        // Assert
        Assert.Equal(5_000_000m, original.BestBid);
        Assert.Equal(4_999_000m, modified.BestBid);
        Assert.NotSame(original, modified);
    }
}
