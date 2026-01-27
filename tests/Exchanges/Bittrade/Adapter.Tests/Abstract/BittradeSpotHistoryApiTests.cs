using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
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
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using Xunit;
using NormalizeRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeSpotHistoryApiTests
{
    [Fact]
    public async Task GetOrdersAsync_LimitApplied()
    {
        var orders = new[]
        {
            new BittradeOpenOrder(
                Symbol: new Symbol("BTC/JPY"),
                Key: new OrderKey(OrderIdKind.AcceptanceId, "id-1"),
                Side: Side.Buy,
                OrderType: OrderType.Limit,
                Size: new Size(0.1m),
                OutstandingSize: new Size(0.1m),
                ExecutedSize: new Size(0m),
                Price: new Price(100m)),
            new BittradeOpenOrder(
                Symbol: new Symbol("BTC/JPY"),
                Key: new OrderKey(OrderIdKind.AcceptanceId, "id-2"),
                Side: Side.Sell,
                OrderType: OrderType.Limit,
                Size: new Size(0.2m),
                OutstandingSize: new Size(0.2m),
                ExecutedSize: new Size(0m),
                Price: new Price(101m))
        };
        var trading = new StubNormalizedTradingApi(orders, Array.Empty<BittradeExecutionNormalized>());
        var api = new BittradeSpotHistoryApi(trading, accountId: "account");

        var call = await api.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<Page<OrderSnapshotItem>>.Ok>(call.Result);

        Assert.Single(ok.Response.Items);
        Assert.Equal(1, ok.Response.Meta.AppliedLimit);
        Assert.Equal(1, ok.Response.Meta.ReturnedCount);
    }

    [Fact]
    public async Task GetExecutionsAsync_LimitApplied()
    {
        var snapshot = EmptySnapshot();
        var executions = new[]
        {
            new BittradeExecutionNormalized(
                "1",
                BittradeOrderSide.Buy,
                100m,
                0.1m,
                DateTimeOffset.UtcNow.AddMinutes(-1),
                snapshot,
                new Dictionary<string, JsonElement>()),
            new BittradeExecutionNormalized(
                "2",
                BittradeOrderSide.Sell,
                101m,
                0.2m,
                DateTimeOffset.UtcNow,
                snapshot,
                new Dictionary<string, JsonElement>())
        };
        var trading = new StubNormalizedTradingApi(Array.Empty<BittradeOpenOrder>(), executions);
        var api = new BittradeSpotHistoryApi(trading, accountId: "account");

        var call = await api.GetExecutionsCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<Page<ExecutionItem>>.Ok>(call.Result);

        Assert.Single(ok.Response.Items);
        Assert.Equal(1, ok.Response.Meta.AppliedLimit);
        Assert.Equal(1, ok.Response.Meta.ReturnedCount);
    }

    private sealed class StubNormalizedTradingApi : IBittradeNormalizedTradingApi
    {
        private readonly IReadOnlyList<BittradeOpenOrder> _openOrders;
        private readonly IReadOnlyList<BittradeExecutionNormalized> _executions;

        public StubNormalizedTradingApi(
            IReadOnlyList<BittradeOpenOrder> openOrders,
            IReadOnlyList<BittradeExecutionNormalized> executions)
        {
            _openOrders = openOrders;
            _executions = executions;
        }

        public Task<Call<NormalizeRequests.PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
            NormalizeRequests.PostOrdersPlaceRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                new BittradeOrderResult(new OrderKey(OrderIdKind.AcceptanceId, "dummy"), AcceptanceId: "dummy")));

        public Task<Call<NormalizeRequests.GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.GetOrdersRequest(),
                (IReadOnlyList<BittradeOrderSummaryNormalized>)Array.Empty<BittradeOrderSummaryNormalized>()));

        public Task<Call<NormalizeRequests.PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey),
                new BittradeCancelResult(true)));

        public Task<Call<NormalizeRequests.PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
            NormalizeRequests.PostOrdersBatchCancelRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeCancelResult(true)));

        public Task<Call<NormalizeRequests.PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
            NormalizeRequests.PostOrdersBatchCancelOpenOrdersRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeCancelResult(true)));

        public Task<Call<NormalizeRequests.GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new NormalizeRequests.GetOpenOrdersRequest(symbol), _openOrders));

        public Task<Call<NormalizeRequests.GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.GetOrderRequest(symbol, orderKey),
                new BittradeOrderStatus(
                    ProductCode: "BTC_JPY",
                    Key: orderKey,
                    Status: OrderState.Active,
                    ExecutedSize: new Size(0m),
                    OutstandingSize: new Size(0.1m),
                    Price: new Price(100m),
                    AveragePrice: new Price(100m))));

        public Task<Call<NormalizeRequests.GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
            NormalizeRequests.GetOrdersMatchResultsByOrderIdRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                (IReadOnlyList<BittradeExecutionNormalized>)Array.Empty<BittradeExecutionNormalized>()));

        public Task<Call<NormalizeRequests.GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
            Symbol symbol,
            int? limit = null,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new NormalizeRequests.GetAccountExecutionsRequest(symbol, limit), _executions));

        public Task<Call<NormalizeRequests.PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
            NormalizeRequests.PostWithdrawApiCreateRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        public Task<Call<NormalizeRequests.PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
            NormalizeRequests.PostRetailOrderPlaceRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeRetailOrderResult(0, null, true, null)));

        public Task<Call<NormalizeRequests.GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
            NormalizeRequests.GetRetailOrderListRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                (IReadOnlyList<BittradeRetailOrderEntryNormalized>)Array.Empty<BittradeRetailOrderEntryNormalized>()));

        public Task<Call<NormalizeRequests.GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
            NormalizeRequests.GetRetailOrderDetailByOrderIdRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall<NormalizeRequests.GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>(request, null));

        public Task<Call<NormalizeRequests.PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
            NormalizeRequests.PostRetailOrderHistoryRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                (IReadOnlyList<BittradeRetailOrderEntryNormalized>)Array.Empty<BittradeRetailOrderEntryNormalized>()));

        public Task<Call<NormalizeRequests.PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
            NormalizeRequests.PostRetailOrderDetailRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall<NormalizeRequests.PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>(request, null));

        public Task<Call<NormalizeRequests.PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
            NormalizeRequests.PostRetailOrderCreateRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeRetailOrderResult(0, null, true, null)));

        public Task<Call<NormalizeRequests.PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
            NormalizeRequests.PostRetailOrderCancelByOrderIdRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeRetailOrderResult(0, null, true, null)));

        public Task<Call<NormalizeRequests.PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
            NormalizeRequests.PostWithdrawVirtualByAddressIdCreateRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        public Task<Call<NormalizeRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            NormalizeRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        public Task<Call<NormalizeRequests.PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            NormalizeRequests.PostWithdrawVirtualByWithdrawIdCancelRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
        {
            var meta = CallMeta.CreateInternal("Normalized", "StubNormalizedTradingApi");
            return new Call<TReq, TResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<TResponse>.Ok(response),
                Meta: meta);
        }
    }

    private static JsonElement EmptySnapshot()
    {
        using var doc = JsonDocument.Parse("{}");
        return doc.RootElement.Clone();
    }
}
