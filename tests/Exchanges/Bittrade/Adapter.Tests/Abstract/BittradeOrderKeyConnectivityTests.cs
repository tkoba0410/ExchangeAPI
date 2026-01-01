using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Spec.CallCommon;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingNormalizedTradingApi
        {
            Order = CreateOrderStatus("1001")
        };
        var api = new BittradeTradingApi(trading);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var status = await api.GetOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.Equal("1001", trading.LastOrderKey?.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingNormalizedTradingApi();
        var api = new BittradeTradingApi(trading);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var result = await api.CancelOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.True(result.IsSuccess);
        Assert.Equal("1002", trading.LastOrderKey?.Value);
    }

    private static OrderStatus CreateOrderStatus(string id) =>
        new(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, id),
            Status: OrderState.Completed,
            ExecutedSize: new Size(0.01m),
            OutstandingSize: new Size(0m),
            Price: new Price(100m),
            AveragePrice: new Price(100m));

    private sealed class RecordingNormalizedTradingApi : IBittradeNormalizedTradingApi
    {
        public OrderKey? LastOrderKey { get; private set; }
        public Symbol? LastSymbol { get; private set; }
        public OrderStatus Order { get; init; } = CreateOrderStatus("default");
        private static readonly BittradeNormalizedRequest DefaultRequest =
            new BittradeNormalizedRequest("test", new Dictionary<string, string?>());

        public Task<OrderResult> PlaceOrderAsync(OrderRequest request, CancellationToken ct = default) =>
            throw new System.NotSupportedException();

        public Task<CancelResult> CancelOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(new CancelResult(true));
        }

        public Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(Symbol symbol, CancellationToken ct = default) =>
            throw new System.NotSupportedException();

        public Task<OrderStatus> GetOrderAsync(Symbol symbol, OrderKey orderKey, CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(Order with { Key = orderKey });
        }

        public Task<BittradeNormalizedCall<OrderResult, System.Text.Json.JsonElement>> PlaceOrderCallAsync(
            OrderRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "dummy"), AcceptanceId: "dummy")));

        public Task<BittradeNormalizedCall<CancelResult, System.Text.Json.JsonElement>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(new CancelResult(true)));
        }

        public Task<BittradeNormalizedCall<IReadOnlyList<OpenOrder>, System.Text.Json.JsonElement>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall<IReadOnlyList<OpenOrder>>(Array.Empty<OpenOrder>()));

        public Task<BittradeNormalizedCall<OrderStatus, System.Text.Json.JsonElement>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(Order with { Key = orderKey }));
        }

        private static BittradeNormalizedCall<TResponse, System.Text.Json.JsonElement> MakeOkCall<TResponse>(TResponse response) =>
            new(
                DefaultRequest,
                new Ok<TResponse, System.Text.Json.JsonElement>(response, 200),
                new CallMeta(DateTimeOffset.UtcNow, TimeSpan.Zero, null));
    }
}
