using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Common.UseCases;

// Compile-only samples. These are not executed during build.
// Real API calls require API keys and network access.

var symbol = new Symbol("BTC/JPY");
var parsed = Symbol.Parse("ETH/JPY");

var acceptanceKey = new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-123");
var exchangeKey = new OrderKey(OrderIdKind.ExchangeOrderId, "ORDER-456");

var pollingOptions = new PollingOptions(TimeSpan.FromSeconds(1), 30)
{
    NotFoundPolicy = NotFoundPolicy.Continue
};

ITradingApi trading = new FakeTradingApi();

// Example flow (no external calls):
var openOrders = await trading.GetOrdersAsync(symbol);
var key = openOrders[0].Key;

await trading.GetOrderAsync(symbol, key);
await trading.CancelOrderAsync(symbol, key);

await OrderPolling.WaitForOrderAsync(trading, symbol, acceptanceKey, pollingOptions, CancellationToken.None);

_ = parsed;
_ = exchangeKey;

sealed class FakeTradingApi : ITradingApi
{
    public Task<OrderResult> PlaceLimitOrderAsync(Symbol symbol, Side side, Size size, Price price, CancellationToken cancellationToken = default) =>
        Task.FromResult(new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-TEST"), AcceptanceId: "ACCEPT-TEST"));

    public Task<OrderResult> PlaceMarketOrderAsync(Symbol symbol, Side side, Size size, CancellationToken cancellationToken = default) =>
        Task.FromResult(new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-TEST"), AcceptanceId: "ACCEPT-TEST"));

    public Task<OrderResult> PlaceStopOrderAsync(Symbol symbol, Side side, Size size, Price triggerPrice, CancellationToken cancellationToken = default) =>
        Task.FromResult(new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-TEST"), AcceptanceId: "ACCEPT-TEST"));

    public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default) =>
        Task.FromResult(new CancelResult(true));

    public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default)
    {
        var order = new OpenOrder(
            ExchangeCode.None,
            symbol,
            new OrderKey(OrderIdKind.AcceptanceId, "ACCEPT-TEST"),
            Side.Buy,
            OrderType.Market,
            new Size(0.01m),
            new Size(0.01m),
            new Size(0m),
            Price: new Price(100m));
        return Task.FromResult<IReadOnlyList<OpenOrder>>(new[] { order });
    }

    public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new OrderStatus(
            ProductCode: "BTC_JPY",
            Key: orderKey,
            Status: OrderState.Active,
            ExecutedSize: new Size(0m),
            OutstandingSize: new Size(0.01m),
            Price: new Price(100m),
            AveragePrice: null));
    }
}
