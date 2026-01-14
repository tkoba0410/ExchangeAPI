using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
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
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.History;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Contracts.Common.CallCommon;
using Xunit;
using NormalizeRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Requests;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeSpotHistoryApiTests
{
    [Fact]
    public async Task GetOrdersAsync_LimitApplied()
    {
        var orders = new[]
        {
            new OpenOrder(
                ExchangeCode.Bittrade,
                new Symbol("BTC/JPY"),
                new OrderKey(OrderIdKind.AcceptanceId, "id-1"),
                Side.Buy,
                OrderType.Limit,
                new Size(0.1m),
                new Size(0.1m),
                new Size(0m),
                new Price(100m)),
            new OpenOrder(
                ExchangeCode.Bittrade,
                new Symbol("BTC/JPY"),
                new OrderKey(OrderIdKind.AcceptanceId, "id-2"),
                Side.Sell,
                OrderType.Limit,
                new Size(0.2m),
                new Size(0.2m),
                new Size(0m),
                new Price(101m))
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
                "buy",
                100m,
                0.1m,
                DateTimeOffset.UtcNow.AddMinutes(-1),
                snapshot,
                new Dictionary<string, JsonElement>()),
            new BittradeExecutionNormalized(
                "2",
                "sell",
                101m,
                0.2m,
                DateTimeOffset.UtcNow,
                snapshot,
                new Dictionary<string, JsonElement>())
        };
        var trading = new StubNormalizedTradingApi(Array.Empty<OpenOrder>(), executions);
        var api = new BittradeSpotHistoryApi(trading, accountId: "account");

        var call = await api.GetExecutionsCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<Page<ExecutionItem>>.Ok>(call.Result);

        Assert.Single(ok.Response.Items);
        Assert.Equal(1, ok.Response.Meta.AppliedLimit);
        Assert.Equal(1, ok.Response.Meta.ReturnedCount);
    }

    private sealed class StubNormalizedTradingApi : IBittradeNormalizedTradingApi
    {
        private readonly IReadOnlyList<OpenOrder> _openOrders;
        private readonly IReadOnlyList<BittradeExecutionNormalized> _executions;

        public StubNormalizedTradingApi(
            IReadOnlyList<OpenOrder> openOrders,
            IReadOnlyList<BittradeExecutionNormalized> executions)
        {
            _openOrders = openOrders;
            _executions = executions;
        }

        public Task<Call<NormalizeRequests.PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
            OrderRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.PlaceOrderRequest(request),
                new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "dummy"), AcceptanceId: "dummy")));

        public Task<Call<NormalizeRequests.CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.CancelOrderRequest(symbol, orderKey),
                new CancelResult(true)));

        public Task<Call<NormalizeRequests.GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new NormalizeRequests.GetOpenOrdersRequest(symbol), _openOrders));

        public Task<Call<NormalizeRequests.GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new NormalizeRequests.GetOrderRequest(symbol, orderKey),
                new OrderStatus(
                    ProductCode: "BTC_JPY",
                    Key: orderKey,
                    Status: OrderState.Active,
                    ExecutedSize: new Size(0m),
                    OutstandingSize: new Size(0.1m),
                    Price: new Price(100m),
                    AveragePrice: new Price(100m))));

        public Task<Call<NormalizeRequests.GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
            Symbol symbol,
            int? limit = null,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new NormalizeRequests.GetAccountExecutionsRequest(symbol, limit), _executions));

        private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
        {
            var meta = new CallMeta(
                Layer: "Normalized",
                Component: "StubNormalizedTradingApi",
                Tags: null,
                Children: null);
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
