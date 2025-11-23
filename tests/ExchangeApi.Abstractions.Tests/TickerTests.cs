using System;
using ExchangeApi.Abstractions.Dtos;
using Xunit;

namespace ExchangeApi.Abstractions.Tests;

public class TickerTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var symbol         = "BTC/JPY";
        var bestBidPrice   = 5_000_000m;
        var bestAskPrice   = 5_001_000m;
        decimal? lastPrice = 5_000_500m;
        decimal? volume    = 1.2345m;
        var timestamp      = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var ticker = new Ticker(
            symbol,
            bestBidPrice,
            bestAskPrice,
            lastPrice,
            volume,
            timestamp);

        // Assert
        Assert.Equal(symbol,       ticker.Symbol);
        Assert.Equal(bestBidPrice, ticker.BestBidPrice);
        Assert.Equal(bestAskPrice, ticker.BestAskPrice);
        Assert.Equal(lastPrice,    ticker.LastTradedPrice);
        Assert.Equal(volume,       ticker.Volume);
        Assert.Equal(timestamp,    ticker.Timestamp);
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
            1.2345m,
            new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        // Act
        var modified = original with { BestBidPrice = 4_999_000m };

        // Assert
        Assert.Equal(5_000_000m, original.BestBidPrice);
        Assert.Equal(4_999_000m, modified.BestBidPrice);
        Assert.NotSame(original, modified);
    }
}
