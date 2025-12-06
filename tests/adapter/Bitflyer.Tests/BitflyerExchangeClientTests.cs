using System;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Adapter.Bitflyer.Facade;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Adapter.Bitflyer.Tests.Fakes;
using Xunit;


namespace ExchangeApi.Adapter.Bitflyer.Tests
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

        [Fact]
        public async Task GetBalancesAsync_ReturnsMappedBalances()
        {
            var rawTicker = new BitflyerTickerRaw { ProductCode = "BTC_JPY" };
            var balances = new[]
            {
                new BitflyerBalanceResponse { CurrencyCode = "JPY", Amount = 10000m, Available = 8000m },
                new BitflyerBalanceResponse { CurrencyCode = "BTC", Amount = 1.5m, Available = 1.2m },
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(balances);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi);

            var result = await client.GetBalancesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Currency == "JPY" && b.Amount == 10000m && b.Available == 8000m);
            Assert.Contains(result, b => b.Currency == "BTC" && b.Amount == 1.5m && b.Available == 1.2m);
        }

        [Fact]
        public async Task GetOpenPositionsAsync_ReturnsMappedPositions()
        {
            var rawTicker = new BitflyerTickerRaw { ProductCode = "BTC_JPY" };
            var positions = new[]
            {
                new BitflyerPositionResponse
                {
                    ProductCode = "BTC_JPY",
                    Side = "BUY",
                    Size = 0.01m,
                    Price = 3000000m,
                    OpenDate = new DateTime(2025, 1, 1),
                    Pnl = 1000m
                }
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>(), positions: positions);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi);

            var result = await client.GetOpenPositionsAsync("BTC_JPY");

            Assert.Single(result);
            var pos = result[0];
            Assert.Equal("BTC_JPY", pos.ProductCode);
            Assert.Equal(OrderSide.Buy, pos.Side);
            Assert.Equal(0.01m, pos.Size);
            Assert.Equal(3000000m, pos.Price);
            Assert.Equal(1000m, pos.Pnl);
        }

        [Fact]
        public async Task GetCollateralAsync_ReturnsMappedCollateral()
        {
            var rawTicker = new BitflyerTickerRaw { ProductCode = "BTC_JPY" };
            var collateral = new BitflyerCollateralResponse
            {
                Collateral = 100000m,
                OpenPositionPnl = 2000m,
                RequireCollateral = 50000m,
                KeepRate = 1.2m
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>(), collateral: collateral);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi);

            var result = await client.GetCollateralAsync();

            Assert.Equal(collateral.Collateral, result.Amount);
            Assert.Equal(collateral.OpenPositionPnl, result.OpenPositionPnl);
            Assert.Equal(collateral.RequireCollateral, result.RequireCollateral);
            Assert.Equal(collateral.KeepRate, result.KeepRate);
        }

        [Fact]
        public async Task CancelOrderAsync_NullResponse_Throws()
        {
            var rawTicker = new BitflyerTickerRaw { ProductCode = "BTC_JPY" };
            var publicApi = new FakeBitflyerPublicApi(rawTicker, new BitflyerBoardRaw { Bids = Array.Empty<BitflyerBoardEntryRaw>(), Asks = Array.Empty<BitflyerBoardEntryRaw>() });
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var tradingApi = new NullCancelTradingApi();
            var client = new BitflyerExchangeClient(publicApi, accountApi, tradingApi);

            await Assert.ThrowsAsync<ExchangeApiException>(() =>
                client.CancelOrderAsync("BTC_JPY", "id-1"));
        }

        [Fact]
        public async Task CancelAllOrdersAsync_NullResponse_Throws()
        {
            var rawTicker = new BitflyerTickerRaw { ProductCode = "BTC_JPY" };
            var publicApi = new FakeBitflyerPublicApi(rawTicker, new BitflyerBoardRaw { Bids = Array.Empty<BitflyerBoardEntryRaw>(), Asks = Array.Empty<BitflyerBoardEntryRaw>() });
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var tradingApi = new NullCancelTradingApi();
            var client = new BitflyerExchangeClient(publicApi, accountApi, tradingApi);

            await Assert.ThrowsAsync<ExchangeApiException>(() =>
                client.CancelAllOrdersAsync("BTC_JPY"));
        }

        private sealed class NullCancelTradingApi : IBitflyerPrivateTradingApi
        {
            public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult(new BitflyerSendChildOrderResponse());

            public Task<BitflyerEmptyResponse> CancelChildOrderAsync(BitflyerCancelChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<BitflyerEmptyResponse>(null!);

            public Task<BitflyerEmptyResponse> CancelAllChildOrdersAsync(BitflyerCancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<BitflyerEmptyResponse>(null!);
        }
    }
}
