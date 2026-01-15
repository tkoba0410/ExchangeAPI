using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Application.UseCases;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.UseCases;

public sealed class OrderPollingTests
{
    [Fact]
    public async Task WaitForOrderAsync_ReturnsLastStatus_WhenMaxAttemptsReached()
    {
        var api = new FakeTradingApi(_ => new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
            Status: OrderState.Active,
            ExecutedSize: new Size(0m),
            OutstandingSize: new Size(1m),
            Price: new Price(100m),
            AveragePrice: null));

        var options = new PollingOptions(TimeSpan.Zero, 3);
        var result = await OrderPolling.WaitForOrderAsync(
            api,
            new Symbol("BTC/JPY"),
            new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
            options,
            CancellationToken.None);

        var ok = Assert.IsType<CallResult<OrderStatus>.Ok>(result.Result);
        Assert.Equal(OrderState.Active, ok.Response.Status);
        Assert.Equal(3, api.CallCount);
    }

    [Fact]
    public async Task WaitForOrderAsync_CancelsDuringDelay()
    {
        var api = new FakeTradingApi(_ => new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
            Status: OrderState.Active,
            ExecutedSize: new Size(0m),
            OutstandingSize: new Size(1m),
            Price: new Price(100m),
            AveragePrice: null));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(10));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            OrderPolling.WaitForOrderAsync(
                api,
                new Symbol("BTC/JPY"),
                new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
                new PollingOptions(TimeSpan.FromMilliseconds(50), 5),
                cts.Token));
    }

    [Fact]
    public async Task WaitForOrderAsync_NotFound_ContinuesUntilMaxAttempts()
    {
        var api = new NotFoundTradingApi();
        var options = new PollingOptions(TimeSpan.Zero, 3);

        var result = await OrderPolling.WaitForOrderAsync(
            api,
            new Symbol("BTC/JPY"),
            new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
            options,
            CancellationToken.None);

        var err = Assert.IsType<CallResult<OrderStatus>.Err>(result.Result);
        Assert.IsType<ExchangeOrderNotFoundException>(err.Error.Exception);
        Assert.Equal(3, api.CallCount);
    }

    [Fact]
    public async Task WaitForOrderAsync_NotFound_StopAsNotFound_Throws()
    {
        var api = new NotFoundTradingApi();
        var options = new PollingOptions(TimeSpan.Zero, 3)
        {
            NotFoundPolicy = NotFoundPolicy.StopAsNotFound
        };

        var result = await OrderPolling.WaitForOrderAsync(
            api,
            new Symbol("BTC/JPY"),
            new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
            options,
            CancellationToken.None);

        var err = Assert.IsType<CallResult<OrderStatus>.Err>(result.Result);
        Assert.IsType<ExchangeOrderNotFoundException>(err.Error.Exception);
        Assert.Equal(1, api.CallCount);
    }

    private sealed class FakeTradingApi : ITradingApi
    {
        private readonly Func<OrderKey, OrderStatus> _next;

        public FakeTradingApi(Func<OrderKey, OrderStatus> next)
        {
            _next = next;
        }

        public int CallCount { get; private set; }

        public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            var request = new GetOrderRequest(symbol, orderKey);
            var now = DateTimeOffset.UtcNow;
            var meta = new CallMeta("Contracts", "Test.GetOrder", null, null);
            return Task.FromResult(new Call<GetOrderRequest, OrderStatus>(
                CallId.New(),
                now,
                TimeSpan.Zero,
                request,
                new CallResult<OrderStatus>.Ok(_next(orderKey)),
                meta));
        }

        public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class NotFoundTradingApi : ITradingApi
    {
        public int CallCount { get; private set; }

        public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            var request = new GetOrderRequest(symbol, orderKey);
            var now = DateTimeOffset.UtcNow;
            var meta = new CallMeta("Contracts", "Test.GetOrder", null, null);
            var error = new CallError(
                CallErrorKind.Semantic,
                "Order not found.",
                new ExchangeOrderNotFoundException(ExchangeCode.Sandbox, "GetOrder", symbol.ToString(), orderKey.ToString()));
            return Task.FromResult(new Call<GetOrderRequest, OrderStatus>(
                CallId.New(),
                now,
                TimeSpan.Zero,
                request,
                new CallResult<OrderStatus>.Err(error),
                meta));
        }

        public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
        public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
