using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Domain.UseCases;
using ExchangeApi.Core.Contracts.Errors;
using Xunit;

namespace Common.Tests.UseCases;

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

        Assert.Equal(OrderState.Active, result.Status);
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

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            OrderPolling.WaitForOrderAsync(
                api,
                new Symbol("BTC/JPY"),
                new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
                options,
                CancellationToken.None));

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

        await Assert.ThrowsAsync<ExchangeOrderNotFoundException>(() =>
            OrderPolling.WaitForOrderAsync(
                api,
                new Symbol("BTC/JPY"),
                new OrderKey(OrderIdKind.AcceptanceId, "order-1"),
                options,
                CancellationToken.None));

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

        public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(_next(orderKey));
        }

        public Task<OrderResult> PlaceLimitOrderAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceMarketOrderAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceStopOrderAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class NotFoundTradingApi : ITradingApi
    {
        public int CallCount { get; private set; }

        public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
        {
            CallCount++;
            throw new ExchangeOrderNotFoundException(ExchangeCode.Sandbox, "GetOrder", symbol.ToString(), orderKey.ToString());
        }

        public Task<OrderResult> PlaceLimitOrderAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceMarketOrderAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceStopOrderAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
