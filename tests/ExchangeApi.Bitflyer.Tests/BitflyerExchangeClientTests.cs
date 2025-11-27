using System;
using System.Threading.Tasks;
using ExchangeApi.Abstractions.Dtos;
using ExchangeApi.Abstractions.Errors;
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
                Timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
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
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi);

            // Act
            var ticker = await client.GetTickerAsync(Symbols.BtcJpy);

            Assert.Equal(Symbols.BtcJpy, ticker.Symbol);
            Assert.Equal(raw.BestBid, ticker.BestBid);
            Assert.Equal(raw.BestAsk, ticker.BestAsk);
            Assert.Equal(raw.LastTradedPrice, ticker.LastTradedPrice);
            Assert.Equal(raw.Timestamp /* 正規化 */, ticker.Timestamp);
        }

        [Fact]
        public async Task GetTickerAsync_UnsupportedSymbol_ThrowsSymbolNotSupportedException()
        {
            // Arrange
            var raw = new BitflyerTickerRaw
            {
                ProductCode = "BTC_JPY",
                Timestamp = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
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
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi);

            var ex = await Assert.ThrowsAsync<ExchangeApiException>(async () =>
                await client.GetTickerAsync("ETH/JPY"));

            Assert.IsType<SymbolNotSupportedException>(ex.InnerException);


        }
    }
}
