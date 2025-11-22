using System;
using System.Threading.Tasks;
using ExchangeApi.Abstractions;
using ExchangeApi.Abstractions.Models;
using ExchangeApi.Bitflyer;
using ExchangeApi.Bitflyer.Models;
using ExchangeApi.Bitflyer.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Bitflyer.Tests
{
    public class BitflyerExchangeClientTests
    {
        [Fact]
        public async Task GetTickerAsync_BtcJpy_ReturnsMappedTicker()
        {
            // Arrange
            var raw = new BitflyerTickerRaw
            {
                ProductCode = "BTC_JPY",
                Timestamp = "2024-01-01T00:00:00Z",
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var fakeApi = new FakeBitflyerPublicApi(raw);
            var client = new BitflyerExchangeClient(fakeApi);

            // Act
            Ticker ticker = await client.GetTickerAsync(Symbols.BtcJpy);

            // Assert
            Assert.Equal(Symbols.BtcJpy, ticker.Symbol);
            Assert.Equal(raw.BestBid, ticker.BestBid);
            Assert.Equal(raw.BestAsk, ticker.BestAsk);
            Assert.Equal(raw.LastTradedPrice, ticker.LastTradedPrice);

            Assert.Equal(DateTimeKind.Utc, ticker.TimestampUtc.Kind);
            Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), ticker.TimestampUtc);
        }
        [Fact]
        public async Task GetTickerAsync_UnsupportedSymbol_ThrowsArgumentException()
        {
            // Arrange
            var raw = new BitflyerTickerRaw
            {
                ProductCode = "BTC_JPY",
                Timestamp = "2024-01-01T00:00:00Z",
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var fakeApi = new FakeBitflyerPublicApi(raw);
            var client = new BitflyerExchangeClient(fakeApi);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await client.GetTickerAsync("ETH/JPY");
            });
        }
        [Fact]
        public async Task GetTickerAsync_InvalidTimestamp_ThrowsExchangeApiException()
        {
            // Arrange
            var raw = new BitflyerTickerRaw
            {
                ProductCode = "BTC_JPY",
                Timestamp = "invalid_timestamp",
                TickId = 123,
                BestBid = 100m,
                BestAsk = 101m,
                BestBidSize = 1.0m,
                BestAskSize = 2.0m,
                TotalBidDepth = 10m,
                TotalAskDepth = 20m,
                LastTradedPrice = 100.5m,
                Volume = 123.45m,
                VolumeByProduct = 200.0m
            };

            var fakeApi = new FakeBitflyerPublicApi(raw);
            var client = new BitflyerExchangeClient(fakeApi);

            // Act & Assert
            await Assert.ThrowsAsync<ExchangeApiException>(async () =>
            {
                await client.GetTickerAsync(Symbols.BtcJpy);
            });
        }
    }
}
