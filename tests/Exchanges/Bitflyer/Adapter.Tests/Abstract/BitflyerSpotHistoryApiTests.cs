using System;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using Xunit;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Private.Api;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract
{
    // ContractsのHistory limit契約（AppliedLimit/ReturnedCount/Items.Count一致）を固定する
    public sealed class BitflyerSpotHistoryApiTests
    {
        [Fact]
        public async Task GetOrdersAsync_ReturnsMappedOrders()
        {
            var rawTicker = new RawPublicDtos.Ticker();
            var childOrders = new[]
            {
                new RawPrivateDtos.RawGetChildOrdersResponse
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
                Array.Empty<RawPrivateDtos.BalanceResponse>());
            var fakeTrading = new FakeBitflyerPrivateTradingApi(
                new RawPrivateDtos.RawSendChildOrderResponse(),
                childOrders);
            var raw = new FakeBitflyerPublicApi(rawTicker, new RawPublicDtos.Board { Bids = Array.Empty<RawPublicDtos.BoardEntry>(), Asks = Array.Empty<RawPublicDtos.BoardEntry>() }, fakePrivate, fakeTrading);
            var client = CreateClient(raw);

            var call = await client.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY")));
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
            var rawTicker = new RawPublicDtos.Ticker();
            var childOrders = new[]
            {
                new RawPrivateDtos.RawGetChildOrdersResponse
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
                new RawPrivateDtos.RawGetChildOrdersResponse
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
                Array.Empty<RawPrivateDtos.BalanceResponse>());
            var fakeTrading = new FakeBitflyerPrivateTradingApi(
                new RawPrivateDtos.RawSendChildOrderResponse(),
                childOrders);
            var raw = new FakeBitflyerPublicApi(rawTicker, new RawPublicDtos.Board { Bids = Array.Empty<RawPublicDtos.BoardEntry>(), Asks = Array.Empty<RawPublicDtos.BoardEntry>() }, fakePrivate, fakeTrading);
            var client = CreateClient(raw);

            var call = await client.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
            var ok = Assert.IsType<CallResult<Page<OrderSnapshotItem>>.Ok>(call.Result);

            Assert.Single(ok.Response.Items);
            Assert.Equal(1, ok.Response.Meta.RequestedLimit);
            Assert.Equal(1, ok.Response.Meta.AppliedLimit);
            Assert.Equal(1, ok.Response.Meta.ReturnedCount);
        }

        [Fact]
        public async Task GetExecutions_Limit1_SlicesItemsAndAlignsMeta()
        {
            var rawTicker = new RawPublicDtos.Ticker();
            var executions = new[]
            {
                new RawPrivateDtos.ExecutionPrivateResponse
                {
                    Id = 1,
                    ProductCode = "BTC_JPY",
                    Side = "BUY",
                    Price = 100m,
                    Size = 0.1m,
                    ExecDate = DateTimeOffset.UtcNow.AddMinutes(-1)
                },
                new RawPrivateDtos.ExecutionPrivateResponse
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
                Array.Empty<RawPrivateDtos.BalanceResponse>(),
                executions: executions);
            var fakeTrading = new FakeBitflyerPrivateTradingApi(new RawPrivateDtos.RawSendChildOrderResponse());
            var raw = new FakeBitflyerPublicApi(rawTicker, new RawPublicDtos.Board { Bids = Array.Empty<RawPublicDtos.BoardEntry>(), Asks = Array.Empty<RawPublicDtos.BoardEntry>() }, fakePrivate, fakeTrading);
            var client = CreateClient(raw);

            var call = await client.GetExecutionsCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
            var ok = Assert.IsType<CallResult<Page<ExecutionItem>>.Ok>(call.Result);

            Assert.Single(ok.Response.Items);
            Assert.Equal(1, ok.Response.Meta.RequestedLimit);
            Assert.Equal(1, ok.Response.Meta.AppliedLimit);
            Assert.Equal(1, ok.Response.Meta.ReturnedCount);
        }

        private static BitflyerExchangeClient CreateClient(IBitflyerRawApi raw)
        {
            var markets = BitflyerTestHelpers.CreateResolver();
            var normalized = BitflyerTestHelpers.CreateNormalizedApi(raw, markets);
            return new BitflyerExchangeClient(normalized);
        }

    }
}
