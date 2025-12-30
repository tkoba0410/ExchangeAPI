using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
using RawOrderState = ExchangeApi.Exchanges.Bittrade.Raw.OrderState;
using RawOrderType = ExchangeApi.Exchanges.Bittrade.Raw.OrderType;
using System.Threading;
using System.Threading.Tasks;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Domain.Services;
using ExchangeApi.Contracts.Dtos;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingRawTradingApi
        {
            Order = CreateOrderDetail("1001")
        };
        var api = new BittradeTradingApi(trading, CreateResolver(), accountId: "acc-1");

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var status = await api.GetOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.Equal("1001", trading.LastOrderId);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingRawTradingApi();
        var api = new BittradeTradingApi(trading, CreateResolver(), accountId: "acc-1");

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var result = await api.CancelOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.True(result.IsSuccess);
        Assert.Equal("1002", trading.LastOrderId);
    }

    private static RawOrderDetailResponse CreateOrderDetail(string id) =>
        new(
            Status: "ok",
            Data: new RawOrderDetail(
                Id: new RawOrderId(id),
                RawSymbol: RawSymbol.From("btcjpy"),
                AccountId: "acc-1",
                Amount: "0.01",
                Price: "100",
                State: RawOrderState.Filled,
                Type: RawOrderType.BuyLimit,
                ClientOrderId: null,
                CreatedAt: DateTimeOffset.FromUnixTimeMilliseconds(1),
                FinishedAt: null,
                FilledAmount: "0.01",
                FilledCashAmount: "1",
                Fees: "0"));

    private static IExchangeMarketResolver CreateResolver() =>
        new ExchangeInfoMarketResolver(new StubExchangeInfoApi(new ExchangeInfo(
            new[] { new ExchangeMarketInfo("BTC/JPY", "btcjpy", "Spot") },
            null,
            null,
            null)));

    private sealed class StubExchangeInfoApi : IExchangeInfoApi
    {
        private readonly ExchangeInfo _info;

        public StubExchangeInfoApi(ExchangeInfo info) => _info = info;

        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(_info);
    }

    private sealed class RecordingRawTradingApi : IBittradeRawTradingApi
    {
        public string? LastOrderId { get; private set; }
        public RawOrderDetailResponse Order { get; init; } = CreateOrderDetail("0");

        public Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RawPlaceOrderResponse("ok", new RawOrderId("0")));

        public Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
        {
            LastOrderId = orderId.Value;
            return Task.FromResult(new RawCancelOrderResponse("ok", orderId));
        }

        public Task<RawOpenOrdersResponse> GetOpenOrdersAsync(RawSymbol symbol, string accountId, CancellationToken cancellationToken = default) =>
            Task.FromResult(new RawOpenOrdersResponse("ok", Array.Empty<RawOrderSummary>()));

        public Task<RawOrderDetailResponse> GetOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default)
        {
            LastOrderId = orderId.Value;
            return Task.FromResult(Order);
        }
    }
}
