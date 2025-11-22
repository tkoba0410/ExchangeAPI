using System;
using ExchangeApi.Abstractions;
using ExchangeApi.Abstractions.Models;
using Xunit;

namespace ExchangeApi.Abstractions.Tests
{
    public class TickerTests
    {
        [Fact]
        public void CanCreateTickerWithProperties()
        {
            // Arrange
            var nowUtc = DateTime.UtcNow;

            // Act
            var ticker = new Ticker
            {
                Symbol = Symbols.BtcJpy,
                BestBid = 100m,
                BestAsk = 101m,
                LastTradedPrice = 100.5m,
                TimestampUtc = nowUtc
            };

            // Assert
            Assert.Equal(Symbols.BtcJpy, ticker.Symbol);
            Assert.Equal(100m, ticker.BestBid);
            Assert.Equal(101m, ticker.BestAsk);
            Assert.Equal(100.5m, ticker.LastTradedPrice);
            Assert.Equal(nowUtc, ticker.TimestampUtc);
        }
    }
}
