using System;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Contracts;

public class GetTickerResponseTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var lastPrice = 5_000_500m;
        var timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var ticker = new GetTickerResponse(
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(lastPrice),
            Timestamp: timestamp);

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.Equal(new Price(lastPrice), ticker.LastTradedPrice);
        Assert.Equal(timestamp, ticker.Timestamp);
    }

    [Fact]
    public void WithExpression_CreatesModifiedInstance()
    {
        var original = new GetTickerResponse(
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(5_000_500m),
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var modified = original with { LastTradedPrice = new Price(4_999_000m) };

        Assert.Equal(new Price(5_000_500m), original.LastTradedPrice);
        Assert.Equal(new Price(4_999_000m), modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }

    [Fact]
    public void GetTickerResponse_IsImmutable()
    {
        var original = new GetTickerResponse(
            Symbol: new Symbol("BTC/JPY"),
            LastTradedPrice: new Price(5_000_500m),
            Timestamp: new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var modified = original with { LastTradedPrice = new Price(4_999_000m) };

        Assert.Equal(new Price(5_000_500m), original.LastTradedPrice);
        Assert.Equal(new Price(4_999_000m), modified.LastTradedPrice);
        Assert.NotSame(original, modified);
    }
}
