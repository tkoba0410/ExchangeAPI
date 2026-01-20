using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Errors;
using ExchangeApi.Application.Interfaces;
using ExchangeApi.Application.Trading;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Application.UseCases;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.UseCases;

public sealed class OrderPollingTests
{
    [Fact]
    public async Task WaitForOrderAsync_ReturnsLastStatus_WhenMaxAttemptsReached()
    {
        var api = new FakeTradingApi(_ => new OrderStatusSnapshot(
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

        var ok = Assert.IsType<CallResult<OrderStatusSnapshot>.Ok>(result.Result);
        Assert.Equal(OrderState.Active, ok.Response.Status);
        Assert.Equal(3, api.CallCount);
    }

    [Fact]
    public async Task WaitForOrderAsync_CancelsDuringDelay()
    {
        var api = new FakeTradingApi(_ => new OrderStatusSnapshot(
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

        var err = Assert.IsType<CallResult<OrderStatusSnapshot>.Err>(result.Result);
        Assert.IsType<OrderNotFoundException>(err.Error.Exception);
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

        var err = Assert.IsType<CallResult<OrderStatusSnapshot>.Err>(result.Result);
        Assert.IsType<OrderNotFoundException>(err.Error.Exception);
        Assert.Equal(1, api.CallCount);
    }

    private sealed class FakeTradingApi : IOrderQueryApi
    {
        private readonly Func<OrderKey, OrderStatusSnapshot> _next;

        public FakeTradingApi(Func<OrderKey, OrderStatusSnapshot> next)
        {
            _next = next;
        }

        public int CallCount { get; private set; }

        public Task<Call<GetOrderQuery, OrderStatusSnapshot>> GetOrderCallAsync(
            GetOrderQuery request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            var now = DateTimeOffset.UtcNow;
            var meta = CallMeta.CreateInternal("Contracts", "Test.GetOrder");
            return Task.FromResult(new Call<GetOrderQuery, OrderStatusSnapshot>(
                CallId.New(),
                now,
                TimeSpan.Zero,
                request,
                new CallResult<OrderStatusSnapshot>.Ok(_next(request.OrderKey)),
                meta));
        }
    }

    private sealed class NotFoundTradingApi : IOrderQueryApi
    {
        public int CallCount { get; private set; }

        public Task<Call<GetOrderQuery, OrderStatusSnapshot>> GetOrderCallAsync(
            GetOrderQuery request,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            var now = DateTimeOffset.UtcNow;
            var meta = CallMeta.CreateInternal("Contracts", "Test.GetOrder");
            var error = new CallError(
                CallErrorKind.Semantic,
                "Order not found.",
                new OrderNotFoundException(ExchangeCode.Sandbox, "GetOrder", request.Symbol, request.OrderKey));
            return Task.FromResult(new Call<GetOrderQuery, OrderStatusSnapshot>(
                CallId.New(),
                now,
                TimeSpan.Zero,
                request,
                new CallResult<OrderStatusSnapshot>.Err(error),
                meta));
        }
    }
}
