using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Common.Contract.Dtos;
using Common.Contract.Enums;
using Common.Contract.Errors;
using Exchange.Bitflyer.Abstract;
using Exchange.Bitflyer.Raw;
using RawProductCode = Exchange.Bitflyer.Raw.ProductCode;
using Exchange.Bitflyer.Tests.Fakes;
using Xunit;


namespace Exchange.Bitflyer.Tests
{
    public class BitflyerExchangeClientTests
    {
        [Fact]
        public async Task GetTickerAsync_BtcJpy_ReturnsMappedTicker()
        {
            // Arrange
            var raw = new BitflyerTicker
            {
                ProductCode = RawProductCode.BtcJpy,
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
            var ticker = await client.GetTickerAsync("BTC/JPY");

            Assert.Equal("BTC/JPY", ticker.Symbol);
            Assert.Equal(raw.BestBid, ticker.BestBid);
            Assert.Equal(raw.BestAsk, ticker.BestAsk);
            Assert.Equal(raw.LastTradedPrice, ticker.LastTradedPrice);
            Assert.Equal(raw.Timestamp /* 正規化 */, ticker.Timestamp);
        }

        [Fact]
        public async Task GetTickerAsync_UnsupportedSymbol_ThrowsSymbolNotSupportedException()
        {
            // Arrange
            var raw = new BitflyerTicker
            {
                ProductCode = RawProductCode.BtcJpy,
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
            await client.GetTickerAsync("UNKNOWN/JPY"));

        Assert.IsType<SymbolNotSupportedException>(ex.InnerException);


        }

        [Fact]
        public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
        {
            var rawTicker = new BitflyerTicker
            {
                ProductCode = RawProductCode.BtcJpy,
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

            var boardRaw = new BitflyerBoard
            {
                MidPrice = 100.5m,
                Bids = new[]
                {
                    new BitflyerBoardEntry { Price = 100m, Size = 0.1m },
                },
                Asks = new[]
                {
                    new BitflyerBoardEntry { Price = 101m, Size = 0.2m },
                }
            };

            var fakeApi = new FakeBitflyerPublicApi(rawTicker, boardRaw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new BitflyerSendChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi);

            var board = await client.GetOrderBookAsync("BTC/JPY");

            Assert.Equal(boardRaw.MidPrice, board.MidPrice);
            Assert.Single(board.Bids);
            Assert.Single(board.Asks);
            Assert.Equal(100m, board.Bids[0].Price);
            Assert.Equal(0.1m, board.Bids[0].Size);
        }

        [Fact]
        public async Task GetOrdersAsync_ReturnsMappedOrders()
        {
            var rawTicker = new BitflyerTicker();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new BitflyerBoard { Bids = Array.Empty<BitflyerBoardEntry>(), Asks = Array.Empty<BitflyerBoardEntry>() });

            var childOrders = new[]
            {
                new BitflyerChildOrderResponse
                {
                    ChildOrderId = "JOR-1",
                    ChildOrderAcceptanceId = "JRF-1",
                    ProductCode = RawProductCode.BtcJpy,
                    Side = Side.Buy,
                ChildOrderType = ChildOrderType.Limit,
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

            var result = await client.GetOrdersAsync("BTC_JPY");

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
            var rawTicker = new BitflyerTicker { ProductCode = RawProductCode.BtcJpy };
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
            var rawTicker = new BitflyerTicker { ProductCode = RawProductCode.BtcJpy };
            var positions = new[]
            {
                new BitflyerPositionResponse
                {
                    ProductCode = RawProductCode.BtcJpy,
                    Side = Side.Buy,
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
            var rawTicker = new BitflyerTicker { ProductCode = RawProductCode.BtcJpy };
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
            var rawTicker = new BitflyerTicker { ProductCode = RawProductCode.BtcJpy };
            var publicApi = new FakeBitflyerPublicApi(rawTicker, new BitflyerBoard { Bids = Array.Empty<BitflyerBoardEntry>(), Asks = Array.Empty<BitflyerBoardEntry>() });
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<BitflyerBalanceResponse>());
            var tradingApi = new NullCancelTradingApi();
            var client = new BitflyerExchangeClient(publicApi, accountApi, tradingApi);

            await Assert.ThrowsAsync<ExchangeApiException>(() =>
                client.CancelOrderAsync("BTC_JPY", "id-1"));
        }

        private sealed class NullCancelTradingApi : IBitflyerPrivateTradingApi
        {
            public Task<BitflyerSendChildOrderResponse> PlaceChildOrderAsync(BitflyerSendChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult(new BitflyerSendChildOrderResponse());

            public Task<BitflyerEmptyResponse> CancelChildOrderAsync(BitflyerCancelChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<BitflyerEmptyResponse>(null!);

            public Task<BitflyerEmptyResponse> CancelAllOrdersAsync(BitflyerCancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<BitflyerEmptyResponse>(null!);

            public Task<BitflyerSendParentOrderResponse> SendParentOrderAsync(BitflyerSendParentOrderRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult(new BitflyerSendParentOrderResponse());

            public Task<BitflyerEmptyResponse> CancelParentOrderAsync(BitflyerCancelParentOrderRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult<BitflyerEmptyResponse>(null!);

            public Task<BitflyerWithdrawResponse> RequestWithdrawalAsync(BitflyerWithdrawRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult(new BitflyerWithdrawResponse());
        }
    }
}
