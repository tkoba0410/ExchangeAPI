using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.UseCases;

var trading = new FakeTradingApi();
var symbol = new Symbol("BTC/JPY");
var orderKey = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-123");

var status = await OrderPolling.WaitForOrderAsync(
    trading,
    symbol,
    orderKey,
    new PollingOptions(TimeSpan.FromMilliseconds(10), 3)
    {
        NotFoundPolicy = NotFoundPolicy.Continue
    });

Console.WriteLine($"Order status: {status.Status}");

sealed class FakeTradingApi : ITradingApi
{
    private int _calls;

    public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
    {
        _calls++;
        var state = _calls < 2 ? OrderState.Active : OrderState.Completed;
        return Task.FromResult(new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: orderKey,
            Status: state,
            ExecutedSize: state == OrderState.Completed ? 0.01m : 0m,
            OutstandingSize: state == OrderState.Completed ? 0m : 0.01m,
            Price: 100m,
            AveragePrice: 100m));
    }

    public Task<OrderResult> PlaceLimitOrderAsync(Symbol symbol, Side side, decimal size, decimal price, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<OrderResult> PlaceMarketOrderAsync(Symbol symbol, Side side, decimal size, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<OrderResult> PlaceStopOrderAsync(Symbol symbol, Side side, decimal size, decimal triggerPrice, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
