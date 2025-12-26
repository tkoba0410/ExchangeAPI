using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;
using ExchangeApi.Exchanges.Bitflyer.Wire.Public;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;
using RawChildOrderType = ExchangeApi.Exchanges.Bitflyer.Raw.ChildOrderType;
using RawSide = ExchangeApi.Exchanges.Bitflyer.Raw.Side;
using ContractSide = ExchangeApi.Common.Enums.Side;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using Xunit;


namespace ExchangeApi.Exchanges.Bitflyer.Tests
{
    public class BitflyerExchangeClientTests
    {
        [Fact]
        public async Task GetTickerAsync_BtcJpy_ReturnsMappedTicker()
        {
            // Arrange
            var raw = new Ticker
            {
                ProductCode = new RawProductCode("BTC_JPY"),
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
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi, fakeApi);

            // Act
        var ticker = await client.GetTickerAsync(new Symbol("BTC/JPY"));

        Assert.Equal(new Symbol("BTC/JPY"), ticker.Symbol);
        Assert.Equal(new Price(raw.LastTradedPrice), ticker.LastTradedPrice);
        Assert.Equal(raw.Timestamp /* 正規化 */, ticker.Timestamp);
        }

        [Fact]
        public async Task GetTickerAsync_UnsupportedSymbol_ThrowsSymbolNotSupportedException()
        {
            // Arrange
            var raw = new Ticker
            {
                ProductCode = new RawProductCode("BTC_JPY"),
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
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi, fakeApi);

        await Assert.ThrowsAsync<SymbolNotSupportedException>(() =>
            client.GetTickerAsync(Symbol.Empty));


        }

        [Fact]
        public async Task GetOrderBookAsync_ReturnsMappedOrderBook()
        {
            var rawTicker = new Ticker
            {
                ProductCode = new RawProductCode("BTC_JPY"),
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

            var boardRaw = new Board
            {
                MidPrice = 100.5m,
                Bids = new[]
                {
                    new BoardEntry { Price = 100m, Size = 0.1m },
                },
                Asks = new[]
                {
                    new BoardEntry { Price = 101m, Size = 0.2m },
                }
            };

            var fakeApi = new FakeBitflyerPublicApi(rawTicker, boardRaw);
            var fakePrivateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
            var fakeTradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(fakeApi, fakePrivateApi, fakeTradingApi, fakeApi);

            var board = await client.GetOrderBookAsync(new Symbol("BTC/JPY"));

            Assert.Single(board.Bids);
            Assert.Single(board.Asks);
            Assert.Equal(new Price(100m), board.Bids[0].Price);
            Assert.Equal(new Size(0.1m), board.Bids[0].Size);
        }

        [Fact]
    public async Task GetOrdersAsync_ReturnsMappedOrders()
        {
            var rawTicker = new Ticker();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new Board { Bids = Array.Empty<BoardEntry>(), Asks = Array.Empty<BoardEntry>() });

            var childOrders = new[]
            {
                new ChildOrderResponse
                {
                    ChildOrderId = "JOR-1",
                    ChildOrderAcceptanceId = "JRF-1",
                    ProductCode = new RawProductCode("BTC_JPY"),
                    Side = RawSide.Buy,
                ChildOrderType = RawChildOrderType.Limit,
                    Price = 100m,
                    Size = 0.1m,
                    OutstandingSize = 0.1m,
                    ExecutedSize = 0m
                }
            };

            var fakePrivate = new FakeBitflyerPrivateApi(
                Array.Empty<BalanceResponse>(),
                childOrders: childOrders);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(fakePublic, fakePrivate, fakeTrading, fakePublic);

            var result = await client.GetOrdersAsync(new Symbol("BTC/JPY"));

            Assert.Single(result);
            var order = result[0];
            Assert.Equal(OrderIdKind.AcceptanceId, order.Key.Kind);
            Assert.Equal("JRF-1", order.Key.Value);
            Assert.Equal(ContractSide.Buy, order.Side);
            Assert.Equal(OrderType.Limit, order.OrderType);
            Assert.Equal(new Size(0.1m), order.Size);
        }

        [Fact]
        public async Task GetBalancesAsync_ReturnsMappedBalances()
        {
            var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
            var balances = new[]
            {
                new BalanceResponse { CurrencyCode = "JPY", Amount = 10000m, Available = 8000m },
                new BalanceResponse { CurrencyCode = "BTC", Amount = 1.5m, Available = 1.2m },
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(balances);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi, publicApi);

            var result = await client.GetBalancesAsync();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.Currency == "JPY" && b.Amount == 10000m && b.Available == 8000m);
            Assert.Contains(result, b => b.Currency == "BTC" && b.Amount == 1.5m && b.Available == 1.2m);
        }

        [Fact]
        public async Task GetOpenPositionsAsync_ReturnsMappedPositions()
        {
            var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
            var positions = new[]
            {
                new PositionResponse
                {
                    ProductCode = new RawProductCode("BTC_JPY"),
                    Side = RawSide.Buy,
                    Size = 0.01m,
                    Price = 3000000m,
                    OpenDate = new DateTime(2025, 1, 1),
                    Pnl = 1000m
                }
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>(), positions: positions);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi, publicApi);

            var result = await client.GetOpenPositionsAsync(new Symbol("BTC/JPY"));

            Assert.Single(result);
            var pos = result[0];
            Assert.Equal(new Symbol("BTC/JPY"), pos.Symbol);
            Assert.Equal(ContractSide.Buy, pos.Side);
            Assert.Equal(new Size(0.01m), pos.Size);
            Assert.Equal(new Price(3000000m), pos.Price);
            Assert.Equal(1000m, pos.Pnl);
        }

        [Fact]
        public async Task GetCollateralAsync_ReturnsMappedCollateral()
        {
            var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
            var collateral = new CollateralResponse
            {
                Collateral = 100000m,
                OpenPositionPnl = 2000m,
                RequireCollateral = 50000m,
                KeepRate = 1.2m
            };

            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>(), collateral: collateral);
            var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi, publicApi);

            var result = await client.GetCollateralAsync();

            Assert.Equal(collateral.Collateral, result.Amount);
            Assert.Equal(collateral.OpenPositionPnl, result.OpenPositionPnl);
            Assert.Equal(collateral.RequireCollateral, result.RequireCollateral);
            Assert.Equal(collateral.KeepRate, result.KeepRate);
        }

        [Fact]
        public async Task GetTradingCommissionAsync_ReturnsRawJson()
        {
            var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
            var publicApi = new FakeBitflyerPublicApi(rawTicker);
            var privateApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
            var tradingApi = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = new BitflyerExchangeClient(publicApi, privateApi, tradingApi, publicApi);

            var result = await client.GetTradingCommissionAsync(new Symbol("BTC/JPY"));

            Assert.Equal(JsonValueKind.Object, result.ValueKind);
        }

        [Fact]
        public async Task CancelOrderAsync_NullResponse_Throws()
        {
            var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
            var publicApi = new FakeBitflyerPublicApi(rawTicker, new Board { Bids = Array.Empty<BoardEntry>(), Asks = Array.Empty<BoardEntry>() });
            var accountApi = new FakeBitflyerPrivateApi(Array.Empty<BalanceResponse>());
            var tradingApi = new NullCancelTradingApi();
            var client = new BitflyerExchangeClient(publicApi, accountApi, tradingApi, publicApi);

            await Assert.ThrowsAsync<ExchangeApiException>(() =>
                client.CancelOrderAsync(new Symbol("BTC/JPY"), new OrderKey(OrderIdKind.AcceptanceId, "id-1")));
        }

        private sealed class NullCancelTradingApi : IBitflyerPrivateTradingApi
        {
            public Task<CreateChildOrderResponse> CreateChildOrderAsync(CreateChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult(new CreateChildOrderResponse());

            public Task<EmptyResponse> CancelChildOrderAsync(CancelChildOrderRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<EmptyResponse>(null!);

            public Task<EmptyResponse> CancelAllChildOrdersAsync(CancelAllChildOrdersRequest request, CancellationToken cancellationToken = default)
                => Task.FromResult<EmptyResponse>(null!);

            public Task<CreateParentOrderResponse> CreateParentOrderAsync(CreateParentOrderRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult(new CreateParentOrderResponse());

            public Task<EmptyResponse> CancelParentOrderAsync(CancelParentOrderRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult<EmptyResponse>(null!);

            public Task<CreateWithdrawalResponse> CreateWithdrawalAsync(CreateWithdrawalRequest request, CancellationToken cancellationToken = default) =>
                Task.FromResult(new CreateWithdrawalResponse());
        }
    }
}
