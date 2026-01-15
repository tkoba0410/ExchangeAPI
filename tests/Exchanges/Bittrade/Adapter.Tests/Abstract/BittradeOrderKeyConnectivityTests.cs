using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Api.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

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
        var call = await api.GetOrderCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<OrderStatus>.Ok>(call.Result);
        var status = ok.Response;

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
        var call = await api.CancelOrderCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<CancelResult>.Ok>(call.Result);
        var result = ok.Response;

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

        public Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
            OrderRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new PlaceOrderRequest(request), new OrderResult(new OrderKey(OrderIdKind.AcceptanceId, "dummy"), AcceptanceId: "dummy")));

        public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(new CancelOrderRequest(symbol, orderKey), new CancelResult(true)));
        }

        public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new GetOpenOrdersRequest(symbol), (IReadOnlyList<OpenOrder>)Array.Empty<OpenOrder>()));

        public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(new GetOrderRequest(symbol, orderKey), Order with { Key = orderKey }));
        }

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
            Symbol symbol,
            int? limit = null,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new GetAccountExecutionsRequest(symbol, limit),
                (IReadOnlyList<BittradeExecutionNormalized>)Array.Empty<BittradeExecutionNormalized>()));

        private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
        {
            var meta = new CallMeta(
                Layer: "Normalized",
                Component: "RecordingNormalizedTradingApi",
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
}
