using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;
using RawOrderState = ExchangeApi.Exchanges.Bittrade.Raw.OrderState;
using RawOrderType = ExchangeApi.Exchanges.Bittrade.Raw.OrderType;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Tests;

public sealed class BittradeWireTradingTests
{
    [Fact]
    public void PlaceOrder_StatusNotOk_Throws()
    {
        var raw = new FakeRawTradingApi
        {
            PlaceOrderResponse = new RawPlaceOrderResponse("error", new RawOrderId("1"))
        };

        var api = new BittradeWireTradingApi(raw, "account-id");
        var ex = Assert.Throws<ExchangeApiException>(() =>
            api.PlaceOrderAsync(new BittradeWireCreateOrderRequest("btcjpy", "buy", "buy-limit", 1m, 0.1m))
                .GetAwaiter()
                .GetResult());

        Assert.Equal(ExchangeCode.Bittrade, ex.Exchange);
        Assert.Equal(BittradeWireOperations.Trading.PlaceOrder, ex.Operation);
        Assert.Equal(ExchangeErrorCategory.Request, ex.ErrorCategory);
        Assert.Contains("status=error", ex.Message);
    }

    [Fact]
    public void GetOpenOrders_ParseError_IncludesFieldAndValue()
    {
        var order = new RawOrderSummary(
            Id: new RawOrderId("1"),
            RawSymbol: RawSymbol.From("btcjpy"),
            AccountId: "account-id",
            Amount: "bad",
            Price: "1",
            State: RawOrderState.Submitted,
            Type: RawOrderType.BuyLimit,
            ClientOrderId: null,
            CreatedAt: DateTimeOffset.UtcNow,
            FilledAmount: "0");

        var raw = new FakeRawTradingApi
        {
            OpenOrdersResponse = new RawOpenOrdersResponse("ok", new[] { order })
        };

        var api = new BittradeWireTradingApi(raw, "account-id");
        var ex = Assert.Throws<ExchangeApiException>(() =>
            api.GetOpenOrdersAsync("btcjpy")
                .GetAwaiter()
                .GetResult());

        Assert.Equal(ExchangeCode.Bittrade, ex.Exchange);
        Assert.Equal(BittradeWireOperations.Trading.GetOpenOrders, ex.Operation);
        Assert.Equal(ExchangeErrorCategory.Unknown, ex.ErrorCategory);
        Assert.Contains("amount", ex.Message);
        Assert.Contains("bad", ex.Message);
    }

    private sealed class FakeRawTradingApi : IBittradeRawTradingApi
    {
        public RawPlaceOrderResponse PlaceOrderResponse { get; init; } =
            new("ok", new RawOrderId("1"));

        public RawCancelOrderResponse CancelOrderResponse { get; init; } =
            new("ok", new RawOrderId("1"));

        public RawOpenOrdersResponse OpenOrdersResponse { get; init; } =
            new("ok", Array.Empty<RawOrderSummary>());

        public RawOrderDetailResponse OrderDetailResponse { get; init; } =
            new("ok", null);

        public Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(PlaceOrderResponse);

        public Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
            Task.FromResult(CancelOrderResponse);

        public Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default) =>
            Task.FromResult(OpenOrdersResponse);

        public Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default) =>
            Task.FromResult(OrderDetailResponse);
    }
}
