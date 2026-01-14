using System;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts;

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
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(lastPrice),
            Timestamp: timestamp);

        // Assert
        Assert.Equal(ExchangeCode.None, ticker.Exchange);
        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.Equal(new Price(lastPrice), ticker.LastTradedPrice);
        Assert.Equal(timestamp, ticker.Timestamp);
    }

    [Fact]
    public void WithExpression_CreatesModifiedInstance()
    {
        // Arrange
        var original = new Ticker(
            Exchange: ExchangeCode.None,
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(5_000_500m),
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { LastTradedPrice = new Price(4_999_000m) };

        // Assert
        Assert.Equal(new Price(5_000_500m), original.LastTradedPrice);
        Assert.Equal(new Price(4_999_000m), modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }

    [Fact]
    public void Ticker_IsImmutable()
    {
        // Arrange
        var original = new Ticker(
            Exchange: ExchangeCode.None,
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(5_000_500m),
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        // Act
        var modified = original with { LastTradedPrice = new Price(4_999_000m) };

        // Assert
        Assert.Equal(new Price(5_000_500m), original.LastTradedPrice);
        Assert.Equal(new Price(4_999_000m), modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }
}
