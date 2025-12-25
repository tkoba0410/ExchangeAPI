using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis;
using ExchangeApi.Exchanges.Bittrade.Wire.Private;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommonSymbol = ExchangeApi.Common.Types.Symbol;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Services;
using ExchangeApi.Contracts.Dtos;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var wire = new RecordingWireTradingApi
        {
            Order = CreateWireOrder("1001")
        };
        var api = new BittradeTradingApi(wire, CreateResolver());

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var status = await api.GetOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.Equal("1001", wire.LastOrderId);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var wire = new RecordingWireTradingApi();
        var api = new BittradeTradingApi(wire, CreateResolver());

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var result = await api.CancelOrderAsync(new CommonSymbol("BTC/JPY"), key);

        Assert.True(result.IsSuccess);
        Assert.Equal("1002", wire.LastOrderId);
    }

    private static BittradeWireOrder CreateWireOrder(string id) =>
        new(
            OrderId: id,
            Symbol: "btcjpy",
            Side: "buy",
            Type: "buy-limit",
            State: "filled",
            Price: 100m,
            Size: 0.01m,
            FilledSize: 0.01m,
            OutstandingSize: 0m,
            CreatedAt: DateTimeOffset.FromUnixTimeMilliseconds(1));

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

    private sealed class RecordingWireTradingApi : IBittradeWireTradingApi
    {
        public string? LastOrderId { get; private set; }
        public BittradeWireOrder Order { get; init; } = CreateWireOrder("0");

        public Task<BittradeWireOrder> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default) =>
            Task.FromResult(Order);

        public Task CancelOrderAsync(string orderId, CancellationToken ct = default)
        {
            LastOrderId = orderId;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<BittradeWireOpenOrder>>(new List<BittradeWireOpenOrder>());

        public Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default)
        {
            LastOrderId = orderId;
            return Task.FromResult(Order);
        }
    }
}
