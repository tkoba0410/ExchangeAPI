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

        [Fact]
        public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
        {
            var rawTicker = new BitflyerTickerRaw
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

            var boardRaw = new BitflyerBoardRaw
            {
                MidPrice = 100.5m,
                Bids = new[]
                {
                    new BitflyerBoardEntryRaw { Price = 100m, Size = 0.1m },
                },
                Asks = new[]
                {
                    new BitflyerBoardEntryRaw { Price = 101m, Size = 0.2m },
                }
            };

            var fakeApi = new FakeBitflyerPublicApi(rawTicker, boardRaw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi);

            var board = await client.GetOrderBookAsync(Symbols.BtcJpy);

            Assert.Equal(boardRaw.MidPrice, board.MidPrice);
            Assert.Single(board.Bids);
            Assert.Single(board.Asks);
            Assert.Equal(100m, board.Bids[0].Price);
            Assert.Equal(0.1m, board.Bids[0].Size);
        }

        [Fact]
        public async Task GetOpenOrdersAsync_ReturnsMappedOrders()
        {
            var rawTicker = new BitflyerTickerRaw();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new BitflyerBoardRaw { Bids = Array.Empty<BitflyerBoardEntryRaw>(), Asks = Array.Empty<BitflyerBoardEntryRaw>() });

            var childOrders = new[]
            {
                new BitflyerChildOrderResponse
                {
                    ChildOrderId = "JOR-1",
                    ChildOrderAcceptanceId = "JRF-1",
                    ProductCode = "BTC_JPY",
                    Side = "BUY",
                    ChildOrderType = "LIMIT",
                    Price = 100m,
                    Size = 0.1m,
                    OutstandingSize = 0.1m,
                    ExecutedSize = 0m
                }
            };

            var fakePrivate = new FakeBitflyerPrivateApi(
                Array.Empty<BitflyerBalanceResponse>(),
                childOrders: childOrders);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(fakePublic, fakePrivate, fakeTrading);

            var result = await client.GetOpenOrdersAsync("BTC_JPY");

            Assert.Single(result);
            var order = result[0];
            Assert.Equal("JOR-1", order.OrderId);
            Assert.Equal(OrderSide.Buy, order.Side);
            Assert.Equal(OrderType.Limit, order.OrderType);
            Assert.Equal(0.1m, order.Size);
        }
    }
}
