using System;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Contracts.Common.CallCommon;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using Xunit;
using RawTicker = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models.Ticker;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract
{
    // ContractsのHistory limit契約（AppliedLimit/ReturnedCount/Items.Count一致）を固定する
    public sealed class BitflyerSpotHistoryApiTests
    {
        [Fact]
        public async Task GetOrdersAsync_ReturnsMappedOrders()
        {
            var rawTicker = new RawTicker();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new Board { Bids = Array.Empty<BoardEntry>(), Asks = Array.Empty<BoardEntry>() });

            var childOrders = new[]
            {
                new ChildOrderResponse
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
                Array.Empty<BalanceResponse>(),
                childOrders: childOrders);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = CreateClient(fakePublic, fakePrivate, fakeTrading);

            var call = await client.History.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY")));
            var ok = Assert.IsType<CallResult<Page<OrderSnapshotItem>>.Ok>(call.Result);
            var result = ok.Response.Items;

            Assert.Single(result);
            var order = result[0];
            Assert.Equal("JRF-1", order.OrderId);
            Assert.Equal(Side.Buy, order.Side);
            Assert.Equal(OrderSnapshotType.Limit, order.OrderType);
            Assert.Equal(new Size(0.1m), order.Size);
        }

        [Fact]
        public async Task GetOrders_Limit1_SlicesItemsAndAlignsMeta()
        {
            var rawTicker = new RawTicker();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new Board { Bids = Array.Empty<BoardEntry>(), Asks = Array.Empty<BoardEntry>() });

            var childOrders = new[]
            {
                new ChildOrderResponse
                {
                    ChildOrderAcceptanceId = "JRF-1",
                    ProductCode = "BTC_JPY",
                    Side = "BUY",
                    ChildOrderType = "LIMIT",
                    Price = 100m,
                    Size = 0.1m,
                    OutstandingSize = 0.1m,
                    ExecutedSize = 0m
                },
                new ChildOrderResponse
                {
                    ChildOrderAcceptanceId = "JRF-2",
                    ProductCode = "BTC_JPY",
                    Side = "SELL",
                    ChildOrderType = "LIMIT",
                    Price = 101m,
                    Size = 0.2m,
                    OutstandingSize = 0.2m,
                    ExecutedSize = 0m
                }
            };

            var fakePrivate = new FakeBitflyerPrivateApi(
                Array.Empty<BalanceResponse>(),
                childOrders: childOrders);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = CreateClient(fakePublic, fakePrivate, fakeTrading);

            var call = await client.History.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
            var ok = Assert.IsType<CallResult<Page<OrderSnapshotItem>>.Ok>(call.Result);

            Assert.Single(ok.Response.Items);
            Assert.Equal(1, ok.Response.Meta.RequestedLimit);
            Assert.Equal(1, ok.Response.Meta.AppliedLimit);
            Assert.Equal(1, ok.Response.Meta.ReturnedCount);
        }

        [Fact]
        public async Task GetExecutions_Limit1_SlicesItemsAndAlignsMeta()
        {
            var rawTicker = new RawTicker();
            var fakePublic = new FakeBitflyerPublicApi(rawTicker, new Board { Bids = Array.Empty<BoardEntry>(), Asks = Array.Empty<BoardEntry>() });

            var executions = new[]
            {
                new ExecutionPrivateResponse
                {
                    Id = 1,
                    ProductCode = "BTC_JPY",
                    Side = "BUY",
                    Price = 100m,
                    Size = 0.1m,
                    ExecDate = DateTimeOffset.UtcNow.AddMinutes(-1)
                },
                new ExecutionPrivateResponse
                {
                    Id = 2,
                    ProductCode = "BTC_JPY",
                    Side = "SELL",
                    Price = 101m,
                    Size = 0.2m,
                    ExecDate = DateTimeOffset.UtcNow
                }
            };

            var fakePrivate = new FakeBitflyerPrivateApi(
                Array.Empty<BalanceResponse>(),
                executions: executions);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new CreateChildOrderResponse());
            var client = CreateClient(fakePublic, fakePrivate, fakeTrading);

            var call = await client.History.GetExecutionsCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
            var ok = Assert.IsType<CallResult<Page<ExecutionItem>>.Ok>(call.Result);

            Assert.Single(ok.Response.Items);
            Assert.Equal(1, ok.Response.Meta.RequestedLimit);
            Assert.Equal(1, ok.Response.Meta.AppliedLimit);
            Assert.Equal(1, ok.Response.Meta.ReturnedCount);
        }

        private static BitflyerExchangeClient CreateClient(
            IBitflyerRawMarketDataApi marketData,
            IBitflyerPrivateApi accountApi,
            IBitflyerRawPrivateTradingApi tradingApi)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
            var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
            var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, accountApi, markets);

            return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedTrading);
        }

    }
}
